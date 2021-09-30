using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskDb.Library
{
    public static class Document
    {
        public const string Grain = @"1.4.2.Reducing Graininess
When you take pictures in low - light conditions or with a very fast exposure time, the camera does not get enough data to make good estimates of the true color at each pixel, and consequently the resulting image looks grainy.You can “smooth out” the graininess by blurring the image, but then you will also lose sharpness.There are a couple of approaches that may give better results.Probably the best, if the graininess is not too bad, is to use the filter called Selective Blur, setting the blurring radius to 1 or 2 pixels.The other approach is to use the Despeckle filter.This has a nice preview, so you can play with the settings and try to find some that give good results.When graininess is really bad, though, it is often very difficult to fix by anything except heroic measures(i.e., retouching with paint tools).
";
        public const string Norm = @"Normalize
This tool(it is really a plug -in) is useful for underexposed images: it adjusts the whole image uniformly until the brightest point is right at the saturation limit, and the darkest point is black.The downside is that the amount of brightening is determined entirely by the lightest and darkest points in the image, so even one single white pixel and / or one single black pixel will make normalization ineffective.
";
        public const string GimpDoc = @"Normalize
This tool(it is really a plug -in) is useful for underexposed images: it adjusts the whole image uniformly until the brightest point is right at the saturation limit, and the darkest point is black.The downside is that the amount of brightening is determined entirely by the lightest and darkest points in the image, so even one single white pixel and / or one single black pixel will make normalization ineffective.

Equalize
This is a very powerful adjustment that tries to spread the colors in the image evenly across the range of possible intensities.In some cases the effect is amazing, bringing out contrasts that are very difficult to get in any other way; but more commonly, it just makes the image look weird.Oh well, it only takes a moment to try.

Color Enhance
Help me, what exactly does this do? Obviously it makes some things more saturated.

Stretch Contrast
This is like “Normalize”, except that it operates on the red, green, and blue channels independently.It often has the useful effect of reducing color casts.

Auto Levels
This is done by activating the Levels tool(Tools → Color Tools → Levels in the image menu), clicking on the image to bring up the tool dialog, and then pressing the Auto button near the center of the dialog.You will see a preview of the result; you must press Okay for it to take effect.Pressing Cancel instead will cause your image to revert to its previous state.


If you can find a point in the image that ought to be perfect white, and a second point that ought to be perfect black, then you can use the Levels tool to do a semi-automatic adjustment that will often do a good job of fixing both brightness and colors throughout the image.First, bring up the Levels tool as previously described.Now, look down near the bottom of the Layers dialog for three buttons with symbols on them that look like eye - droppers(at least, that is what they are supposed to look like).The one on the left, if you mouse over it, shows its function to be “Pick Black Point”. Click on this, then click on a point in the image that ought to be black–really truly perfectly black, not just sort of dark–and watch the image change.Next, click on the rightmost of the three buttons( “Pick White Point” ), and then click a point in the image that ought to be white, and once more watch the image change.If you are happy with the result, click the Okay button otherwise Cancel.

1.4.  Adjusting Sharpness
1.4.1.  Unblurring
If the focus on the camera is not set perfectly, or the camera is moving when the picture is taken, the result is a blurred image.If there is a lot of blurring, you probably won't be able to do much about it with any technique, but if there is only a moderate amount, you should be able to improve the image.

The most generally useful technique for sharpening a fuzzy image is called the Unsharp Mask. In spite of the rather confusing name, which derives from its origins as a technique used by film developers, its result is to make the image sharper, not “unsharp”. It is a plug-in, and you can access it as Filters->Enhance->Unsharp Mask in the image menu.There are two parameters, “Radius” and “Amount”. The default values often work pretty well, so you should try them first. Increasing either the radius or the amount increases the strength of the effect.Don't get carried away, though: if you make the unsharp mask too strong, it will amplify noise in the image and also give rise to visible artifacts where there are sharp edges.

[Tip]   Tip
Sometimes using Unsharp Mask can cause color distortion where there are strong contrasts in an image.When this happens, you can often get better results by decomposing the image into separate Hue-Saturation-Value (HSV) layers, and running Unsharp Mask on the Value layer only, then recomposing. This works because the human eye has much finer resolution for brightness than for color.See the sections on Decompose and Compose for more information.

Next to ""Unsharp Mask"" in the Filters menu is another filter called Sharpen, which does similar things. It is a little easier to use but not nearly as effective: our recommendation is that you ignore it and go straight to Unsharp Mask.

In some situations, you may be able to get useful results by selectively sharpening specific parts of an image using the Blur or Sharpen tool from the Toolbox, in ""Sharpen"" mode.This allows you to increase the sharpness in areas by painting over them with any paintbrush.You should be restrained about this, though, or the results will not look very natural: sharpening increases the apparent sharpness of edges in the image, but also amplifies noise.

1.4.2.Reducing Graininess
When you take pictures in low - light conditions or with a very fast exposure time, the camera does not get enough data to make good estimates of the true color at each pixel, and consequently the resulting image looks grainy.You can “smooth out” the graininess by blurring the image, but then you will also lose sharpness.There are a couple of approaches that may give better results.Probably the best, if the graininess is not too bad, is to use the filter called Selective Blur, setting the blurring radius to 1 or 2 pixels.The other approach is to use the Despeckle filter.This has a nice preview, so you can play with the settings and try to find some that give good results.When graininess is really bad, though, it is often very difficult to fix by anything except heroic measures(i.e., retouching with paint tools).

1.4.3.Softening
Every so often you have the opposite problem: an image is too crisp.The solution is to blur it a bit: fortunately blurring an image is much easier than sharpening it.Since you probably don't want to blur it very much, the simplest method is to use the “Blur” plug-in, accessed via Filters->Blur->Blur from the image menu. This will soften the focus of the image a little bit. If you want more softening, just repeat until you get the result you desire.";
    }
}
