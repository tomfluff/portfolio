Pictures saved here for monitoring and reference only, it does not work!
If a new picture needs to be added or an olt one changed;
It must be changed (added) to all the platform specific projects in the following locations:
1. iOS : Under 'Resources' folder
2. Android : Under 'Resources\Drawable' folder
3. UWP : Uder the project root (i.e. /Sunnyday.Client.UWP)

The name should be the same all across, after that it can be used freely from the ahred code.
E.g. if the picture is called 'image.png' then use:
<Image Source="image.png"></Image>

On top of that the build action (accessed from the Preferences pane) should be:
1. iOS : BundleResource
2. Android : AndroidResource
3. UWP : Content


Good Luck!