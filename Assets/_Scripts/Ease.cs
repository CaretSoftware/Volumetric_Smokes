using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ease {
    private const float c1 = 1.70158f;
    private const float c2 = c1 * 1.525f;
    private const float c3 = c1 + 1f;
    private const float c4 = (2f * Mathf.PI) / 3f;
    private const float n1 = 7.5625f;
    private const float d1 = 2.75f;

    /// <summary>
    /// Use with LerpUnclamped
    /// https://easings.net/#easeInBack
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InBack(float x) {
        return c3 * x * x * x - c1 * x * x;
    }
    
    /// <summary>
    /// https://easings.net/#easeInCubic 
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InCubic(float x) {
        return x * x * x;
	}
    
    /// <summary>
    /// https://easings.net/#easeInCirc
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InCirc(float x) {
        return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
    }

    /// <summary>
    /// https://easings.net/#easeInQuad
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>   
    public static float InQuad(float x) {
        return x * x;
    }

    /// <summary>
    /// https://easings.net/#easeInQuart
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>   
    public static float InQuart(float x) {
        return x * x * x * x;
    }

    /// <summary>
    /// https://easings.net/#easeInQuint
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>   
    public static float InQuint(float x) {
        return x * x * x * x * x;
    }

    /// <summary>
    /// https://easings.net/#easeInExpo
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>    
    public static float InExpo(float x) {
        // TODO test if handles negative numbers TODO
        return x == 0 ? 0 : Mathf.Pow(2f, 10f * x - 10f);
    }

    
    /// <summary>
    /// https://easings.net/#easeInSine
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>    
    public static float InSine(float x) {
        return 1f - Mathf.Cos((x * Mathf.PI) / 2f);
	}

    
    /// <summary>
    /// Use with LerpUnclamped
    /// https://easings.net/#easeOutBack
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutBack(float x) {
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1
                * Mathf.Pow(x - 1f, 2f);
    }
    
    /// <summary>
    /// https://easings.net/#easeOutBounce
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutBounce(float x){

        if (x < 1.0f / d1) {
            return n1 * x * x;
        } else if (x < 2.0f / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        } else if (x < 2.5f / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        } else {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }

    /// <summary>
    /// can't handle x = 0!
    /// https://easings.net/#easeOutCirc
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>    
    public static float OutCirc(float x) {
        return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
    }

    /// <summary>
    /// https://easings.net/#easeOutCubic
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>    
    public static float OutCubic(float x) {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    /// <summary>
    /// Use with LerpUnclamped
    /// https://easings.net/#easeOutElastic
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutElastic(float x) {
        return x == 0
                ? 0
                : x == 1
                        ? 1
                        : Mathf.Pow(2.0f, -10f * x)
                                * Mathf.Sin((x * 10.0f - 0.75f) * c4) + 1.0f;
    }
    
    /// <summary>
    /// Use with LerpUnclamped
    /// https://easings.net/#easeInElastic
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InElastic(float x) {

        return x == 0
            ? 0
            : x == 1
                ? 1
                : -Mathf.Pow(2.0f, 10.0f * x - 10.0f) 
                        * Mathf.Sin((x * 10.0f - 10.75f) * c4);
    }

    /// <summary>
    /// https://easings.net/#easeOutExpo
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutExpo(float x) {
        return x == 1
                ? 1
                : x > 0.0f
                        ? 1 - Mathf.Pow(2f, -10f * x)
                        : -(1 - Mathf.Pow(2f, -10f * -x));
    }

    /// <summary>
    /// https://easings.net/#easeOutQuint
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutQuint(float x) {
        return 1 - Mathf.Pow(1f - x, 5f);
    }

    /// <summary>
    /// https://easings.net/#easeOutSine
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float OutSine(float x) {
        return Mathf.Sin((x * Mathf.PI) / 2f);
	}

    
    /// <summary>
    /// Use with LerpUnclamped
    /// https://easings.net/#easeInOutBack
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutBack(float x) {
        return x < 0.5f
                ? (Mathf.Pow(2f * x, 2f)
                        * ((c2 + 1f) * 2f * x - c2)) / 2f
                : (Mathf.Pow(2f * x - 2f, 2f)
                        * ((c2 + 1f) * (x* 2f - 2f) + c2) + 2f) / 2f;
    }

    /// <summary>
    /// https://easings.net/#easeInOutCirc
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutCirc(float x) {
        return x < 0.5f
                ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f
                : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
    }

    /// <summary>
    /// https://easings.net/#easeInOutCubic
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutCubic(float x) {
        return x < 0.5f
                ? 4f * x * x * x
                : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    /// <summary>
    /// https://easings.net/#easeInOutQuad
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutQuad(float x) {
        return x < 0.5f
                ? 2.0f * x * x
                : 1.0f - Mathf.Pow(-2.0f * x + 2.0f, 2.0f) / 2.0f;
    }

    /// <summary>
    /// https://easings.net/#easeInOutQuint
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutQuint(float x) {
        return x < 0.5f
                ? 16f * x * x * x * x * x
                : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
    }

    /// <summary>
    /// https://easings.net/#easeInOutSine
    /// </summary>
    /// <param name="x">usually a over time linearly increasing value between 0.0f and 1.0f</param>
    /// <returns>a value that goes from 0.0f to 1.0f, increasing non-linearly</returns>
    public static float InOutSine(float x) {
        return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
    }
}