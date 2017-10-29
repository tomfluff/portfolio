using System.Diagnostics;
using Xamarin.Forms;

namespace SunnyDay.Client.Core.Utils.DataConvert
{

    public static class Uv
    {
        public static int UV_ToPublic(int i)
        {
            switch (i)
            {
                case 1:
                    return 0;
                case 2:
                    return 2;
                case 3:
                    return 5;
                case 4:
                    return 8;
                case 5:
                    return 10;
                default:
                    return -1;
            }
        }

        public static int UV_ToLocal(int i)
        {
            switch (i)
            {
                case 0:
                    return 1;
                case 1:
                case 2:
                    return 2;
                case 3:
                case 4:
                case 5:
                    return 3;
                case 6:
                case 7:
                case 8:
                    return 4;
                case 9:
                case 10:
                case 11:
                    return 5;
                default:
                    return 0;
            }
        }

        public static Color UV_ToColor(int i)
        {
            switch (i)
            {
                case 2:
                    return Color.FromHex("#1fba89");
                case 3:
                    return Color.FromHex("#e1ed07");
                case 4:
                    return Color.FromHex("#fcb900");
                case 5:
                    return Color.FromHex("#bf0b0b");
                default:
                    return Color.Transparent;
            }
        }

        public static string UV_ToDescription(int i)
        {
            switch (i)
            {
                case 1:
                    return "None";
                case 2:
                    return "Low";
                case 3:
                    return "Medium";
                case 4:
                    return "High";
                case 5:
                    return "Extreme";
                default:
                    return "Unknown";
            }
        }

        public static double CalcTimeToBurnInSeconds(int skinTone, int spfLevel, int uvLevel, double weatherAltitude)
        {
            var uv = (1 + (int)(weatherAltitude / 305.0) * 0.0487804) * uvLevel;
            Debug.WriteLine($"> Actualy UV={uv}, BaseBurnRate={SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToBaseBurnRate(skinTone)}, SpfProtectionRate={SunnyDay.Client.Core.Utils.DataConvert.Spf.Spf_ToProtectionRate(spfLevel)}");
            var resMinutes = (SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToBaseBurnRate(skinTone) / uv) * SunnyDay.Client.Core.Utils.DataConvert.Spf.Spf_ToProtectionRate(spfLevel);
            Debug.WriteLine($"> Minutes to burn={resMinutes}");
            var res = (int)(resMinutes * 60 + 0.5);
            return res;
        }
    }

    public static class Spf
    {
        public static double Spf_ToProtectionRate(int i)
        {
            switch (i)
            {
                case 15:
                    return 1.3;
                case 30:
                    return 7.5;
                case 35:
                    return 8.5;
                case 40:
                    return 9.5;
                case 45:
                    return 1.3;
                case 50:
                    return 12.4;
                default:
                    return 1;
            }
        }

        public static Color Spf_ToTextColor(int i)
        {
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return Color.Black;
                default:
                    return Color.White;
            }
        }
    }

    public static class SkinTone
    {
        public static string SkinTone_ToDescription(int i)
        {
            switch (i)
            {
                case 0:
                    return "Minimally Pigmented";
                case 1:
                    return "Pale Peach";
                case 2:
                    return "Peach";
                case 3:
                    return "Light Brown";
                case 4:
                    return "Moderate Brown";
                case 5:
                    return "Dark Brown";
                case 6:
                    return "Deeply Pigmented";
                default:
                    return "Unknown";
            }
        }

        public static string SkiTone_ToRemarks(int i)
        {
            switch (i)
            {
                case 0:
                    return "Tend to burn in a matter of minutes. Likely albino, white hair, pale skin.";
                case 1:
                    return "Always burns, never tans. Likely with blond or red hair, blue eyes and freckles.";
                case 2:
                    return
                        "Usually burns, tans minimally. Likely with blond or red hair, and blue, green or hazle eyes.";
                case 3:
                    return "Sometimes mild burn, tans uniformly. Any hair or eye color.";
                case 4:
                    return "Burns minimally, always tans well.";
                case 5:
                    return "Very rarely burns, tans very easily.";
                case 6:
                    return "Never burns, never tans.";
                default:
                    return "You did not take a skin photo yet!";
            }
        }

        public static Color SkinTone_ToColor(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.FromHex("#fdf6f4");
                case 1:
                    return Color.FromHex("#efded4");
                case 2:
                    return Color.FromHex("#ebd0ba");
                case 3:
                    return Color.FromHex("#e7ba85");
                case 4:
                    return Color.FromHex("#c78e5b");
                case 5:
                    return Color.FromHex("#8e6639");
                case 6:
                    return Color.FromHex("#432a1e");
                default:
                    return Color.Transparent;
            }
        }

        public static int SkinTone_ToBaseBurnRate(int i)
        {
            switch (i)
            {
                case 0:
                    return 33;
                case 1:
                    return 67;
                case 2:
                    return 100;
                case 3:
                    return 200;
                case 4:
                    return 300;
                case 5:
                    return 400;
                case 6:
                    return 500;
                default:
                    return 0;
            }
        }

        public static int SkinTone_ToSpfRecommendation(int i)
        {
            switch (i)
            {
                case 0:
                case 1:
                    return 50;
                case 2:
                    return 40;
                case 3:
                case 4:
                    return 30;
                default:
                    return 15;
            }
        }
    }
}