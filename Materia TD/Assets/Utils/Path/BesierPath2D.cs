using UnityEngine;
using System.Collections;

public class BesierPath2D {
    private const int TIME_CORRECTION_STEPS_COUNT = 20;

    public AnimationCurve[] curves;

    private float _finishTime;
    public float FinishTime
    {
        get { return _finishTime; }
    }

    private bool dirty; 

    private float _speed = 1;
    public float Speed
    {
        get { return _speed; }
        set
        {
            if (value > 0)
            {
                _speed = value;
                dirty = true;
            }
        }
    }

    private float _tangents = 1;
    public float Tangents
    {
        get { return _tangents; }
        set
        {
            if (value >= 0)
            {
                _tangents = value;
                dirty = true;
            }
        }
    }

    private Vector3[] _pathPoints;
    public Vector3[] PathPoints
    {
        get { return _pathPoints; }
        set
        {
            if (value != null && value.Length > 1)
            {
                _pathPoints = value;
                dirty = true;
            }
        }
    }

    private float _pieceLength = 1;
    public float PieceLength
    {
        get { return _pieceLength; }
        set
        {
            if (value > 0)
            {
                _pieceLength = value;
                dirty = true;
            }
        }
    }


    public bool IsValid
    {
        get
        {
            return _pathPoints != null;
        }
    }


    private int _initialTangentSign = 1;
    public bool InitialTangentIsPositive
    {
        get { return _initialTangentSign > 0; }
        set 
        { 
            _initialTangentSign = value ? 1 : -1;
            dirty = true;
        }
    }

    public void Update()
    {
        if (!IsValid)
        {
            Debug.LogError("You must set path points before update!!");
            return;
        }

        curves = new AnimationCurve[2];
        curves[0] = new AnimationCurve();
        curves[1] = new AnimationCurve();

        int lineStart = 0;
        int lineEnd = 1;
        float curveTime = 0;
        int tangentSign = _initialTangentSign;
        float lineTangent = 0;

        while(lineEnd < _pathPoints.Length) 
        {
            Vector2 dir = _pathPoints[lineEnd] - _pathPoints[lineStart];
            int currAxis = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 0 : 1;
            int offAxis = currAxis == 0 ? 1 : 0;
            int currDirSign = dir[currAxis] > 0 ? 1 : -1;

            int newDirSign = 0;
            for (lineEnd++; lineEnd < _pathPoints.Length; lineEnd++)
            {
                Vector2 newDir = _pathPoints[lineEnd] - _pathPoints[lineEnd - 1];
                int newAxis = Mathf.Abs(newDir.x) > Mathf.Abs(newDir.y) ? 0 : 1;
                if (newAxis != currAxis)
                {
                    newDirSign = (int)Mathf.Sign(newDir[newAxis]);
                    break;
                }
            }
            lineEnd--;



            if(lineStart != lineEnd) 
            {
                float distance = Mathf.Abs((_pathPoints[lineEnd][currAxis] - _pathPoints[lineStart][currAxis]));
                int partsCount = Mathf.RoundToInt(distance / _pieceLength);
                if (partsCount < 1) partsCount = 1;
                if((partsCount & 1) != 0 && newDirSign == tangentSign
                    || (partsCount & 1) == 0 && newDirSign != tangentSign)
                {
                    partsCount++;
                } 
                float pieceLength = distance / partsCount;
                float pieceTime = pieceLength / _speed;
                float timeCorrectionStep = pieceTime / TIME_CORRECTION_STEPS_COUNT;

                float offPosition = _pathPoints[lineStart][offAxis];
                float currPosition = _pathPoints[lineStart][currAxis] + currDirSign*pieceLength;
                
                if (lineStart == 0 )
                {
                    float positionStart = _pathPoints[lineStart][currAxis];
                    lineTangent = currDirSign * _speed;
                    curves[currAxis].AddKey(new Keyframe(curveTime, positionStart, 0, lineTangent));
                    curves[offAxis].AddKey(new Keyframe(curveTime, offPosition, 0, tangentSign * _tangents));
                }

                for (int i = 0; i < partsCount; i++)
                {
                    
                    Keyframe offAxisKey = new Keyframe();
                    Keyframe currAxisKey = new Keyframe();
                    currAxisKey.inTangent = lineTangent;

                    float correctedPieceTime = pieceTime;
                    float correctionTime = curveTime;
                    float predictTime = curveTime + correctedPieceTime;
                    curveTime = predictTime;

                    tangentSign *= -1;
                    if (i == partsCount - 1) 
                    {
                        lineTangent = newDirSign * _speed;

                        if (lineEnd == _pathPoints.Length - 1)
                        {
                            offAxisKey.inTangent = tangentSign;
                            offAxisKey.outTangent = newDirSign * _speed;
                            currAxisKey.outTangent = currDirSign;

                            offAxisKey.time = predictTime;
                            offAxisKey.value = offPosition;
                            curves[offAxis].AddKey(offAxisKey);

                            currAxisKey.time = predictTime;
                            currAxisKey.value = currPosition;
                            curves[currAxis].AddKey(currAxisKey);
                        }
                    }
                    else
                    {
                        offAxisKey.inTangent = _tangents * tangentSign;
                        offAxisKey.outTangent = offAxisKey.inTangent;
                        currAxisKey.outTangent = currAxisKey.inTangent;

                        offAxisKey.time = predictTime;
                        offAxisKey.value = offPosition;
                        curves[offAxis].AddKey(offAxisKey);

                        currAxisKey.time = predictTime;
                        currAxisKey.value = currPosition;
                        curves[currAxis].AddKey(currAxisKey);

                        currPosition += currDirSign * pieceLength;
                    }

                    
                    
                    /*
                    float x0 = curves[0].Evaluate(correctionTime);
                    float y0 = curves[1].Evaluate(correctionTime);
                    float calculatedLength = 0;
                    correctionTime += timeCorrectionStep;
                    predictTime += 0.00001f;
                    while (correctionTime <= predictTime)
                    {
                        float x1 = curves[0].Evaluate(correctionTime);
                        float y1 = curves[1].Evaluate(correctionTime);
                        float deltaX = x1 - x0;
                        float deltaY = y1 - y0;
                        calculatedLength += Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
                        x0 = x1;
                        y0 = y1;
                        correctionTime += timeCorrectionStep;
                    }
                    float correctedPieceTime = calculatedLength / _speed;
                    curveTime += correctedPieceTime;
                    offAxisKey.time = curveTime;
                    currAxisKey.time = curveTime;

                    
                    int lastIdx = curves[0].length-1;
                    curves[currAxis].RemoveKey(lastIdx);
                    curves[currAxis].AddKey(currAxisKey);
                    curves[offAxis].RemoveKey(lastIdx);
                    curves[offAxis].AddKey(offAxisKey);
                     */
                }
                
                tangentSign = currDirSign;

                lineStart = lineEnd++;
            }
        }
        dirty = false;
        _finishTime = curveTime;
    }

    public bool ApplyPosition(Transform transform, float time) 
    {
        if (dirty) Update();
        Vector3 pos = new Vector3(curves[0].Evaluate(time), curves[1].Evaluate(time), 0);
        transform.position = pos;
        return time >= _finishTime;
    }

    public AnimationCurve GetCurve(int idx)
    {
        return curves[idx];
    }
	
}
