using NaughtyBezierCurves;
using UnityEngine;

public static class BezierCurve3DLinear {
  public static Vector3 GetPointOnLinearCurve(float time, BezierPoint3D startPoint, BezierPoint3D endPoint) {
    return GetPointOnLinearCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition);
  }

  public static Vector3 GetPointOnLinearCurve(float time, Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent, Vector3 endTangent) {
    float t = time;
    
    
    float u = 1f - t;
    float t2 = t * t;
    float u2 = u * u;
    float u3 = u2 * u;
    float t3 = t2 * t;

    Vector3 result =
        (u3) * startPosition +
        (3f * u2 * t) * startTangent +
        (3f * u * t2) * endTangent +
        (t3) * endPosition;
        
    //result = Mathf.Pow(1-time,2)*startPosition+
    //(2f*(1f-time)*time*startTangent)*Mathf.Pow(time,1f)*endPosition;

    return result;
  }

  public static Quaternion GetRotationOnLinearCurve(float time, Vector3 up, BezierPoint3D startPoint,
      BezierPoint3D endPoint) {
    return GetRotationOnLinearCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition);
  }

  public static Quaternion GetRotationOnLinearCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent, Vector3 endTangent) {
    Vector3 tangent = GetTangentOnLinearCurve(time, startPosition, endPosition, startTangent, endTangent);
    Vector3 normal = GetNormalOnLinearCurve(time, up, startPosition, endPosition, startTangent, endTangent);

    return Quaternion.LookRotation(tangent, normal);
  }

  public static Vector3 GetTangentOnLinearCurve(float time, BezierPoint3D startPoint, BezierPoint3D endPoint) {
    return GetTangentOnLinearCurve(time, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition);
  }

  public static Vector3 GetTangentOnLinearCurve(float time, Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent, Vector3 endTangent) {
    float t = time;
    float u = 1f - t;
    float u2 = u * u;
    float t2 = t * t;

    Vector3 tangent =
        (-u2) * startPosition +
        (u * (u - 2f * t)) * startTangent -
        (t * (t - 2f * u)) * endTangent +
        (t2) * endPosition;

    return tangent.normalized;
  }

  public static Vector3 GetBinormalOnLinearCurve(float time, Vector3 up, BezierPoint3D startPoint,
      BezierPoint3D endPoint) {
    return GetBinormalOnLinearCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition);
  }

  public static Vector3 GetBinormalOnLinearCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent, Vector3 endTangent) {
    Vector3 tangent = GetTangentOnLinearCurve(time, startPosition, endPosition, startTangent, endTangent);
    Vector3 binormal = Vector3.Cross(up, tangent);

    return binormal.normalized;
  }

  public static Vector3
      GetNormalOnLinearCurve(float time, Vector3 up, BezierPoint3D startPoint, BezierPoint3D endPoint) {
    return GetNormalOnLinearCurve(time, up, startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition);
  }

  public static Vector3 GetNormalOnLinearCurve(float time, Vector3 up, Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent, Vector3 endTangent) {
    Vector3 tangent = GetTangentOnLinearCurve(time, startPosition, endPosition, startTangent, endTangent);
    Vector3 binormal = GetBinormalOnLinearCurve(time, up, startPosition, endPosition, startTangent, endTangent);
    Vector3 normal = Vector3.Cross(tangent, binormal);

    return normal.normalized;
  }

  public static float
      GetApproximateLengthOfLinearCurve(BezierPoint3D startPoint, BezierPoint3D endPoint, int sampling) {
    return GetApproximateLengthOfLinearCurve(startPoint.Position, endPoint.Position, startPoint.RightHandlePosition,
        endPoint.LeftHandlePosition, sampling);
  }

  public static float GetApproximateLengthOfLinearCurve(Vector3 startPosition, Vector3 endPosition,
      Vector3 startTangent,
      Vector3 endTangent, int sampling) {
    float length = 0f;
    Vector3 fromPoint = GetPointOnLinearCurve(0f, startPosition, endPosition, startTangent, endTangent);

    for (int i = 0; i < sampling; i++) {
      float time = (i + 1) / (float)sampling;
      Vector3 toPoint = GetPointOnLinearCurve(time, startPosition, endPosition, startTangent, endTangent);
      length += Vector3.Distance(fromPoint, toPoint);
      fromPoint = toPoint;
    }

    return length;
  }

  public static void GetLinearSegment(this BezierCurve3D curve, float time, out BezierPoint3D startPoint,
      out BezierPoint3D endPoint,
      out float timeRelativeToSegment) {
    startPoint = null;
    endPoint = null;
    timeRelativeToSegment = 0f;

    float subCurvePercent = 0f;
    float totalPercent = 0f;
    float approximateLength = curve.GetApproximateLength();
    int subCurveSampling = (curve.Sampling / (curve.KeyPointsCount - 1)) + 1;

    for (int i = 0; i < curve.KeyPointsCount - 1; i++) {
      subCurvePercent =
          BezierCurve3D.GetApproximateLengthOfCubicCurve(curve.KeyPoints[i], curve.KeyPoints[i + 1], subCurveSampling)
          / approximateLength;
      if (subCurvePercent + totalPercent > time) {
        startPoint = curve.KeyPoints[i];
        endPoint = curve.KeyPoints[i + 1];

        break;
      }

      totalPercent += subCurvePercent;
    }

    if (endPoint == null) {
      // If the evaluated point is very near to the end of the curve we are in the last segment
      startPoint = curve.KeyPoints[curve.KeyPointsCount - 2];
      endPoint = curve.KeyPoints[curve.KeyPointsCount - 1];

      // We remove the percentage of the last sub-curve
      totalPercent -= subCurvePercent;
    }

    timeRelativeToSegment = (time - totalPercent) / subCurvePercent;
  }

  // Protected Methods
}
