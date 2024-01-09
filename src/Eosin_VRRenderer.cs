/*
    VR Renderer for Virt-a-Mate
    by Eosin
    License: CC BY-SA
 
    Uses code from MacGruber's Essentials (SuperShot, SuperResolution, SkyMagicExporter and utility functions), licensed under CC BY-SA.
    https://hub.virtamate.com/resources/macgruber-essentials.160/

    Uses LilyRender360 shader created by Elie Michel, licensed under MIT License.
    https://github.com/eliemichel/LilyRender360
	
    v15

    - Fixed BVH export being inaccurate especially on the arms, hands and spine
    - Fixed end sites of fingers, toes, breasts and face being misaligned with model in BVH export
	- Added option to export BVH with unoriented rest pose if intention is to later reimport a modified motion with VAM BVH player
	>>> The DAZ-standard BVH rest pose (bind pose with relaxed arms) is different from the VAM/DAZ zero-rotation internal rest pose (axis-aligned T-pose). VAM BVH player currently does not account for this difference which leads to inaccurate results when importing a standard BVH with the default DAZ bind pose (same inaccuracy as old v14). The unoriented rest pose does not align with a VAM or DAZ 3D model but produces correct result when loaded into VAM BVH player. The unoriented rest skeleton could also be used as a retargeting target in an external application if the intention is to load an authored motion into BVH player.
	- Fixed exported BVH FPS interpreted as one less by Blender importer due to rounding down instead of rounding to nearest
	- Fixed BVH export could output very small values in exponential notation
	
    v14

    - Added BVH animation export so you can edit and utilize VaM mocap, behavior and animations externally.
    - The export respects all bone morphs on the rig and you get a 1:1 match with VaM obj export T-pose and can reimport via BVH player
    - When exporting to Blender, you need to set BVH import coordinate system to Y up, -Z forward.
    >>> I recommend the Enhanced BVH plugin for Blender, which can export BVH to a different coordinate system (such as VaM) and also apply the BVH to an existing rig
    - When exporting to DAZ and related software, you can set scale to be in centimeters.

    v13
    
    (v13b)
    - Fixed bug with cannot start new recording if recorded full length of specified seconds once
    - Fixed bug with asking to overwrite a frame when using pause after reaching full length of specified seconds
    - Fix wav file name frame number 1 too high
    - Fix rendering one frame too many

    (v13)
    - Added audio recording when recording video. Audio is recorded at the position of the rendering camera (the control holding the plugin), not the main game view. Audio is saved as a WAV file to the video folder.
    > Audio sample range should usually remain at 32768 but if unexpectedly there are popping noises in the recording you may try reducing this by a notch or two.
    - Video and audio recording can now be paused and resumed (within the same game session). Escape pauses recording if the checkbox is ticked, or if you tick the checkbox afterwards and record again.
    > Pausing recording does not pause any audio playback in the scene, you could use freeze motion/sound for this functionality
    > The total recorded audio is written to a new file with each pause, this seems better than waiting for the user to finalize a single audio file
    - fixed a bug with incomplete cleanup on reload/remove plugin
    - Known issue: Improved PoV plugin (hiding head and hair) does not work in this plugin. Simplest solution for VR180 and flat probably to make character bald and place camera just in front of eyes instead of inside

    v12

    - Added support for command buffer effects
        -> Subsurface Scattering Skin is partially broken because of camera-based effects inside the plugin
    - Added support for post processing effects such as MacGruber's PostMagic
        > Some post processing effects don't work correctly in a VR render. This may be partially ameliorated by using "smooth stitching" functionality. If you cannot get good results you have to use post-processing in a video edtor.
        > Some post processing effects don't work correctly with exporting transparency

    v11

    - Added panoramic stereo mode for VR render: Each pixel column is rendered from the correct position, creating seamless accurate stereo in all view directions except directly up and down.
    >>> This option is around 15-30 times slower than the normal method. However, very low VRAM cost also allows maximum rendering quality next to accurate stereo. VaM will be unresponsive while rendering out a frame.
    - Full support for transparency with semi-transparent clothing and materials in all cases thanks to contribution by kuler. Note this new method is slightly slower and slightly increases VRAM usage but better results, the old method is no longer available.
    - Reduced VRAM consumption for all stereo renders by 30-40% and remove reduce vram option (using different method)
    - Added camera target option to automatically make camera look at an object. Works best with an Empty target and parenting it to your target object, since you cannot select a specific sub-control of an atom inside the plugin.
    - Empty sphere of camera and target can now stay visible when empty is deselected to make it possible to select from the scene instead of menu without needing symbols for whole scene
    - Added "start playback unfreezes motion" option for scenes which don't use scene animation for motion
    - Stereo VR preview for normal and panoramic mode is now from center of both eyes instead of left eye, except when using stereoscopic background

    v10
    
    - Added triangular mapping for stereo 360° VR with pivoting eyes. Scene will be built from 3x3 120°+ FOV cameras instead of cube, reducing seams to 3 (1 directly behind viewer, 2 on sides)
    - Custom seam texture and color can be chosen, seam hiding now has parallax which can make it less distracting
    - Issue: some materials and items are invisible when outputting transparency and there is nothing behind them, these items don't seem to write to alpha or z-buffer
        >>> default transparency to off since most people will not use it by default and would rather see semi-transparent material correctly
        >>> you can now choose a background color and use it to chroma key in an external software to preserve transparency in these cases
        >>> preview now reflects user choice in transparency and updates instantly when changing transparency settings
    - Video render progress is now a GUI bar with time left estimation instead of spamming console
    - Empty's sphere is now invisible in preview when using empty to host the plugin

    v9
    
    - You can now render the background directly to the output and don't have to composite in an external editor if you don't need advanced compositing features
    - You can now load a video file as a background source and directly composite it into the output (Note: No support for x265 codec which applies to some high-resolution VR videos)
        - Note: May rarely get into a bugged state where you can't composite with video even after reloading plugin. Have to restart VaM and it should work.
    - You can now load a stereo image or video as a background source and specify the 3D layout
    - Implemented feature "Rotate Eyes Horizontally" for stereo VR. The eye pair will be rotated around a pivot for each of the horizontal directions, so stereopsis will be correct in all directions. However vertical seams will appear, which are more apparent the closer an object is.
        - You can choose to hide the seams with black but this seems to work poorly because of stereoscopy unaccounted for
    - Fixed a bug where errors would be spammed until settings were changed if initial loading of the shader took longer than 1 frame.    
    - Experimental option to reduce VRAM when rendering stereo by using a single cubemap rendered twice per frame. Performance -33%
    - change over-under 360 layout to top-right bottom-left
    - Render progress information

    v8

    - You can now preview the VR render in real time, at the quality level of the final render. This should make it much easier to position things accurately and judge impact of cubemap resolution, smooth stitching and MSAA settings.
    - You can now load a background image into the preview window. This makes it easy to composite a render into existing VR or flat footage by matching lighting and positioning, or FOV for flat content.
    - Correction to v6 note: smooth stitching actually improves results in all cases, but the improvement can be quite subtle in VR for some scenes. But it is apparent when adjusting smooth stitching in the real time preview.
    - Preview is 2:1 aspect ratio for 360 degree VR
    - Info text to vertical middle since the UI is getting too big
    
    v7
    
    - Added transparency support for VR and flat renders. This allows compositing multiple VR videos together or compositing Virt-a-Mate VR footage into real-life VR footage.
        Note: Loading any scene with PostMagic disables transparency everywhere until you restart VaM
    - Preview window can stay open permanently for positioning scene elements relative to camera
    - Improved crosshair and added borders to preview window so it's clearly visible in empty scenes used when rendering transparent footage
    - Fixed last small bug with field of view on preview camera when changing aspect ratio in VR context
    - Reduce default preview FOV to 120

    v6
    
    - fixed video recording not actually stopping automatically for non-integer recording times
    - added preview resizing and preview crosshair
    - preview window is now always square when rendering VR
    - added support for LilyRender360 smooth stitching
    - increased default preview FOV to 140 to better estimate VR view
    - default MSAA to off, filename empty
    - remove infobox due to lack of space

    v5

    - Fixed incorrect field of view calculation for preview and flat renders
    - Fixed preview window in wrong color space due to Unity bug
    - Preview window remains open during render to verify correct orientation

    1.3
    
    - Added 2:1 resolutions and more options for 1:1 resolutions

    1.2
    
    - Now using Unity capture time setting for frame rate control, leaving physics rate untouched

    1.1

    - Added flat video recording, with supersampling by MacGruber.
    - Mute all sound while recording.
    - Added preview window.
    - Added hotkeys.
    - Fixed non-standard resolution aspect ratios
    - Reduce VRAM consumption slightly.
    - Improve memory estimate (maybe)
    - Added warnings for texture sizes and VRAM consumption exceeding capabilities of GPU.

    1.0

    Intitial release

---------------------
    performance

    // 8k 2048 2MSAA -> 123 frames per minute -> 487 ms per frame
    // cubemap conversion: 127 ms per frame = 25%
    // non-encoding total: 154ms
    // endoding jpeg: 333ms = 70%
    // total jpeg: 487
    // endoding png: 1331ms = 90%
    // total png: 1485
 */

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Diagnostics;
using MeshVR;
using MVR.FileManagementSecure;
using MacGruber;
using Request = MeshVR.AssetLoader.AssetBundleFromFileRequest;
using UnityEngine.Video;
using System.Threading;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEngine.Audio;

namespace Eosin
{
    public class VRRenderer : MVRScript
    {
        private const int STATE_SETUP = 0;
        private const int STATE_PROCESS = 1;

        private static readonly List<string> RENDERMODE_NAMES = new List<string>() {
            "Flat",
            "VR180 Stereo",
            "VR360 Stereo",
            "VR180 Mono",
            "VR360 Mono",
            "BVH Animations"
        };

        private static readonly List<string> STEREOMODE_NAMES = new List<string>() {
            "Static (Fast, Front View Accurate, Transparency)",
            "Panoramic (Very Slow, All Directions Accurate, Transparency)",
            "120° Triangle Split (Fast, 3 Seams, Most Directions Accurate)",
            "90° Square Split (Fast, 4 Seams, Most Directions Accurate)"
        };

        const int STEREO_STATIC = 0;
        const int STEREO_PANORAMIC = 1;
        const int STEREO_TRIANGLE = 2;
        const int STEREO_SQUARE = 3;

        public static readonly List<string> KERNEL_NAMES = new List<string>() { "Linear (Blurring)", "Lanczos 3 (Sharpening)" };
        public const int KERNEL_LINEAR = 0;
        public const int KERNEL_LANCZOS3 = 1;

        private static readonly List<string> ASPECTRATIO_NAMES = new List<string>() {
            "36:9",
            "32:9",
            "21:9",
            "16:9",
            "16:10",
            "4:3",
            "2:1",
            "1:1"
        };

        private static readonly List<Vector2Int>[] RESOLUTION_VALUES = new List<Vector2Int>[] {
            new List<Vector2Int>() { // 36:9
				new Vector2Int(1600, 400),
                new Vector2Int(3200, 800),
                new Vector2Int(6400, 1600)
            },
            new List<Vector2Int>() { // 32:9
				new Vector2Int(2048, 576),
                new Vector2Int(2560, 720),
                new Vector2Int(3840, 1080),
                new Vector2Int(5120, 1440),
                new Vector2Int(7680, 2160),
            },
            new List<Vector2Int>() { // 21:9
				new Vector2Int(2560, 1080),
                new Vector2Int(3440, 1440),
                new Vector2Int(5120, 2160)
            },
            new List<Vector2Int>() { // 16:9
				new Vector2Int(1280, 720),
                new Vector2Int(1920, 1080),
                new Vector2Int(2560, 1440),
                new Vector2Int(3840, 2160),
                new Vector2Int(5120, 2880),
                new Vector2Int(5888, 3312),
                new Vector2Int(6784, 3816),
                new Vector2Int(7680, 4320),
                new Vector2Int(10240, 5760),
                new Vector2Int(11776, 6624),
                new Vector2Int(15360, 8640)
            },
            new List<Vector2Int>() { // 16:10
				new Vector2Int(1280, 800),
                new Vector2Int(1440, 900),
                new Vector2Int(1920, 1200),
                new Vector2Int(3840, 2400),
                new Vector2Int(7680, 4800)
            },
            new List<Vector2Int>() { // 4:3
				new Vector2Int(800, 600),
                new Vector2Int(1024, 768),
                new Vector2Int(2048, 1536),
                new Vector2Int(4096, 3072),
                new Vector2Int(8192, 6144),
            },
            new List<Vector2Int>() { // 2:1
				new Vector2Int(1280, 640),
                new Vector2Int(1920, 960),
                new Vector2Int(2560, 1280),
                new Vector2Int(3840, 1920),
                new Vector2Int(5120, 2560),
                new Vector2Int(5888, 2944),
                new Vector2Int(6784, 3392),
                new Vector2Int(7680, 3840),
                new Vector2Int(10240, 5120),
                new Vector2Int(11776, 5888),
                new Vector2Int(15360, 7680)
            },
            new List<Vector2Int>() { // 1:1
                new Vector2Int(256, 256),
                new Vector2Int(512, 512),
                new Vector2Int(1024, 1024),
                new Vector2Int(1920, 1920),
                new Vector2Int(2048, 2048),
                new Vector2Int(2560, 2560),
                new Vector2Int(3840, 3840),
                new Vector2Int(4096, 4096),
                new Vector2Int(5120, 5120),
                new Vector2Int(5888, 5888),
                new Vector2Int(6784, 6784),
                new Vector2Int(7680, 7680),
                new Vector2Int(8192, 8192),
                new Vector2Int(10240, 10240),
                new Vector2Int(11776, 11776),
                new Vector2Int(15360, 15360)
            }
        };

        private static readonly List<string>[] RESOLUTION_NAMES = new List<string>[] {
            new List<string>() { // 36:9
				"1600x400",
                "3200x800",
                "6400x1600"
            },
            new List<string>() { // 32:9
				"2048x576",
                "2560x720",
                "3840x1080 (DFHD)",
                "5120x1440 (DQHD)",
                "7680x2160 (DUHD)",
            },
            new List<string>() { // 21:9
				"2560x1080 (WFHD)",
                "3440x1440 (WQHD)",
                "5120x2160 (4K WUHD)"
            },
            new List<string>() { // 16:9
				"1280x720 (HD)",
                "1920x1080 (FHD)",
                "2560x1440 (QHD)",
                "3840x2160 (4K UHD)",
                "5120x2880 (5K)",
                "5888x3312 (6K)",
                "6784x3816 (7K)",
                "7680x4320 (8K UHD)",
                "10240x5760 (10K)",
                "11776x6624 (12K)",
                "15360x8640 (16K)"
            },
            new List<string>() { // 16:10
				"1280x800 (WXGA)",
                "1440x900 (WXGA+)",
                "1920x1200 (WUXGA)",
                "3840x2400 (2x WUXGA)",
                "7680x4800 (4x WUXGA)"
            },
            new List<string>() { // 4:3
				"800x600 (SVGA)",
                "1024x768 (XGA)",
                "2048x1536 (2x XGA)",
                "4096x3072 (4x XGA)",
                "8192x6144 (8x XGA)"
            },
            new List<string>() { // 2:1
				"1280x640",
                "1920x960",
                "2560x1280",
                "3840x1920 (4K)",
                "5120x2560 (5K)",
                "5888x2944 (6K)",
                "6784x3392 (7K)",
                "7680x3840 (8K)",
                "10240x5120 (10K)",
                "11776x5888 (12K)",
                "15360x7680 (16K)"
            },
            new List<string>() { // 1:1
				"256x256",
                "512x512",
                "1024x1024",
                "1920x1920",
                "2048x2048",
                "2560x2560",
                "3840x3840 (4K)",
                "4096x4096",
                "5120x5120 (5K)",
                "5888x5888 (6K)",
                "6784x6784 (7K)",
                "7680x7680 (8K)",
                "8192x8192",
                "10240x10240 (10K)",
                "11776x11776 (12K)",
                "15360x15360 (16K)",
            }
        };

        private static readonly List<int> FRAMERATE_VALUES = new List<int>() { 30, 36, 40, 45, 60, 72, 80, 90, 120, 144, 165, 240, 288 };

        private static readonly List<string> FRAMERATE_NAMES = new List<string>() { "30", "36", "40", "45", "60", "72", "80", "90", "120", "144", "165", "240", "288" };

        public static readonly List<int> MSAA_VALUES = new List<int>() { 1, 2, 4, 8 };
        public static readonly List<string> MSAA_NAMES = new List<string>() { "Off", "2x", "4x", "8x" };
        public static readonly List<string> FORMAT_NAMES = new List<string>() { "PNG (Lossless, Big & Slow)\nTransparency Support", "JPEG (Lossy, Small & Fast)\nNo Transparency" };
        public const int FORMAT_PNG = 0;
        public const int FORMAT_JPG = 1;

        private static readonly int[] DEFAULT_RESOLUTION_IDX = new int[] { 1, 1, 0, 7, 2, 1, 7, 11 };

        private const int DEFAULT_RENDERMODE_IDX = 1;
        private const float DEFAULT_IPD = 64;
        private const int DEFAULT_ASPECTRATIO_IDX = 6;
        private const int DEFAULT_MSAA_IDX = 1;
        private const int DEFAULT_FORMAT = FORMAT_PNG;
        private const int DEFAULT_FRAMERATE_IDX = 4;

        private const int ASPECT_SQUARE = 6;

        private JSONStorableString myMemoryInfo;
        private float myMemoryEstimate = -1.0f;
        private JSONStorableString myDiskSpaceInfo;
        private float myDiskSpaceEstimate = -1.0f;
        private JSONStorableString effectiveFrameRate;
        private int effectiveFrameRateInt = 60;

        private bool bRecordVideo = false;
        private bool bFlatRender = false;
        private bool bStereoRender = true;
        private bool b180Degrees = true;

        RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;
        private int renderModeIdx = DEFAULT_RENDERMODE_IDX;
        private int stereoMode = STEREO_STATIC;
        private int cubemapSideLength = 2048;
        private int selectedCubemapSideLength = 2048;
        private int jpegQuality = 98;
        private float ipd = DEFAULT_IPD;
        private static int secondsToRecord = 10;
        private int frameCounter = 0;
        private static int frameRateInt = 60;
        private int frameRateIdx = DEFAULT_FRAMERATE_IDX;
        string baseFilename = "";
        string myDirectory = "";
        bool generatedFilename = false;
        UserPreferences.PhysicsRate oldFrameRate = UserPreferences.PhysicsRate._60;
        private int framesToSkip = 0;
        private int frameSkipCounter = 0;
        private float flatHorizontalFov = 80f;
        private float flatVerticalFov = 80f;
        private int flatSupersampling = 1;
        public int myKernelMode = KERNEL_LANCZOS3;
        private Vector2Int finalResolution;
        JSONStorableBool VRAMWarningChooser;
        JSONStorableBool muteAudioChooser;
        List<float> audioVolumes;
        List<AudioSource> audioSources; // Note: dictionary doesn't seem to complile in VaM plugin
        int previewResolution = 400;
        JSONStorableBool previewChooser;
        JSONStorableBool previewStaysOpenChooser;
        JSONStorableBool unfreezeOnPlaybackChooser;
        JSONStorableUrl previewBackgroundChooser;
        int vrPreviewFPS = 20;
        float lastVRPreviewUpdate;
        bool cubemapRendered;
        float oldPreviewSize;
        int oldSelectedCubemapSideLength;
        int oldMsaaLevel;
        int oldFlatMsaaLevel;
        int oldRenderMode;
        int oldResolution;
        float oldOverlap;
        float lastResize;
        float previewResizePerSecond = 2;
        bool bRendering;
        int backgroundStereoLayout;
        public static readonly List<string> STEREO_LAYOUTS = new List<string>() { "Mono", "Left Right", "Right Left", "Bottom Top", "Top Bottom" };

        const int BG_MONO = 0;
        const int BG_LEFTRIGHT = 1;
        const int BG_RIGHTLEFT = 2;
        const int BG_DOWNUP = 3;
        const int BG_UPDOWN = 4;

        JSONStorableBool renderBackgroundChooser;
        JSONStorableColor backgroundColor;
        JSONStorableBool hideBackgroundColorOnPreviewOnly;
        JSONStorableBool crosshairChooser;
        JSONStorableBool preserveTransparencyChooser;
        private float previewSize = 0.2f;
        private float previewPadding = 0.005f;
        private int oldCaptureFramerate;
        private float lilyRenderOverlap = 0f;
        JSONStorableFloat frontFovChooser;

        JSONStorableFloat eyePivotDistanceChooser;
        JSONStorableFloat hideSizeChooser;
        JSONStorableFloat hideSeamsSizeChooser;
        JSONStorableFloat hideSeamsParallaxChooser;
        JSONStorableUrl seamTextureUrl;
        JSONStorableColor seamTextureTintChooser;

        private int myAspectRatioIdx = DEFAULT_ASPECTRATIO_IDX;
        private int myResolutionIdx = DEFAULT_RESOLUTION_IDX[DEFAULT_ASPECTRATIO_IDX];
        private int myMsaaLevel = MSAA_VALUES[DEFAULT_MSAA_IDX];
        private int myFileFormat = DEFAULT_FORMAT;

        private FreeControllerV3 mySelectedController = null;
        private bool myNeedSetup = true;

        public static readonly string SCREENSHOT_DIRECTORY = "Saves/VR_Renders/";
        string myLilyRenderBundleURL = null;
        string myDownsampleBundleURL = null;

        VideoPlayer videoPlayer;
        GameObject go;
        JSONStorableBool loopChooser;
        JSONStorableFloat timeChooser;
        UIDynamicSlider timeSlider;
        JSONStorableUrl videoURL;
        JSONStorableBool unpauseVideoOnRenderChooser;
        bool videoNotPausedYet;
        bool shouldAdvanceVideo;
        bool videoPlayerWasPlaying;

        List<float> timestamps;
        float timeStampCollectionDuration = 20f;

        GameObject sphereObject, targetSphereObject;

        float videoTimeOnRenderBegin;
        bool insideUpdate;
        float oldTimeScale;
        bool videoFrameReady = true;
        int frameWaitedFor;
        float frameWaitStartTime;
        Texture2D seamTexture;
        bool renderEndedLastFrame;
        bool sphereLayerChanged;
        JSONStorableBool leaveEmptySphereVisible;
        JSONStorableBool usePostProcessing;
        JSONStorableBool useCommandBuffers;
        bool handledPre, handledPost;
        float panoramicVerticalFov = 75f;
        Transform cameraTarget;
        bool unfrozeOnStart;
        Atom myContainingAtom;
        JSONStorableBool recordAudioChooser;
        JSONStorableBool previewAudioFromCamPos;
        JSONStorableFloat audioSampleRangeChooser;
        const float maxShort = 32768f;
        bool bBvhRender;

        JSONStorableBool pauseVideoChooser;
        JSONStorableBool dazBvhScaleChooser;
        JSONStorableBool bvhStartsAtOrigin;
        JSONStorableBool bvhUseUnorientedSkeleton;
        string lastFilename;

        GameObject originalAudioListener;
        AudioVelocityUpdateMode originalVelocityUpdateMode;

        int delayedInitCounter = 2;

        // global render resources
        Camera renderCam, previewCamera;
        GameObject renderCamParentObj;
        Material sliceEquirectMat, _equirectMat, _equirectMatAlpha, _equirectMatRotate, _equirectMatRotateTriangle, myConvolutionMaterial, alphaDiffMat;
        Texture2D finalOutputTexture;
        RenderTexture equirectL, equirectR;
        RenderTexture flatRenderTex, passTexture, outputRenderTexture;
        RenderTexture flatRenderTexBlack, flatRenderTexWhite, blackBgRenderTex, whiteBgRenderTex;
        RenderTexture[] renderFaces;
        RenderTexture previewTex, previewTexBlack, previewTexWhite;
        Texture2D blackTex, semiTex, clearTex;
        Texture2D previewBackground;

        void DelayedInit()
        {
            originalAudioListener = GameObject.FindObjectOfType<AudioListener>()?.gameObject;
            if (originalAudioListener != null)
            {
                originalVelocityUpdateMode = originalAudioListener.GetComponent<AudioListener>().velocityUpdateMode;
                originalAudioListener.GetComponent<AudioListener>().enabled = false;
                Destroy(originalAudioListener.GetComponent<AudioListener>());
            }
            else
            {
                SuperController.LogMessage("VRRenderer: Note: No existing AudioListener in scene");
                originalVelocityUpdateMode = AudioVelocityUpdateMode.Auto;
            }

            containingAtom.mainController.gameObject.AddComponent<AudioListener>();
            containingAtom.mainController.gameObject.GetComponent<AudioListener>().velocityUpdateMode = originalVelocityUpdateMode;
        }

        public override void Init()
        {
            myNeedSetup = true;

            myLilyRenderBundleURL = Utils.GetPluginPath(this) + "/VRRenderer.shaderbundle";
            Request request = new AssetLoader.AssetBundleFromFileRequest { path = myLilyRenderBundleURL, callback = OnLilyRenderBundleLoaded };
            AssetLoader.QueueLoadAssetBundleFromFile(request);

            myDownsampleBundleURL = Utils.GetPluginPath(this) + "/MacGruber_Convolution.shaderbundle";
            request = new AssetLoader.AssetBundleFromFileRequest { path = myDownsampleBundleURL, callback = OnDownsampleBundleLoaded };
            AssetLoader.QueueLoadAssetBundleFromFile(request);

            myContainingAtom = containingAtom;

            BuildUI();

            timestamps = new List<float>(500);

            Camera.onPreRender += OnPreRenderCallback;
            // Camera.onPostRender += OnPostRenderCallback;

            previewCamera = CreateFlatCamera(containingAtom.mainController.transform);
            previewCamera.clearFlags = CameraClearFlags.SolidColor;
            previewCamera.targetTexture = previewTex;
            previewCamera.fieldOfView = flatVerticalFov;
            SyncCameraTransform();

            clearTex = new Texture2D(1, 1);
            clearTex.SetPixel(0, 0, new Color(0, 0, 0, 0));
            clearTex.Apply();

            blackTex = new Texture2D(1, 1);
            blackTex.SetPixel(0, 0, new Color(0, 0, 0, 1));
            blackTex.Apply();

            semiTex = new Texture2D(1, 1);
            semiTex.SetPixel(0, 0, new Color(0, 0, 0, 0.75f));
            semiTex.Apply();


            Transform origin = containingAtom.mainController.gameObject.transform;
            Transform t;
            if ((t = origin.parent) != null && t.gameObject.name == "reParentObject")
            {
                if ((t = t.Find("object")) != null)
                {
                    if ((t = t.Find("rescaleObject")) != null)
                    {
                        if ((t = t.Find("SphereRender")) != null)
                        {
                            if (t.gameObject.GetComponent<MeshRenderer>() != null)
                            {
                                sphereObject = t.gameObject;

                                int layer = LayerMask.NameToLayer("UI");
                                if ((t.gameObject.layer & layer) != layer)
                                {
                                    sphereLayerChanged = true;
                                    sphereObject.layer |= layer;
                                }
                            }
                        }
                    }
                }
            }

            myNeedSetup = true;
        }

        void BuildUI()
        {
            Utils.OnInitUI(CreateUIElement);

            previewChooser = Utils.SetupToggle(this, "Preview", true, true);
            previewStaysOpenChooser = Utils.SetupToggle(this, "Preview Stays Open", true, true);
            previewAudioFromCamPos = Utils.SetupToggle(this, "Preview Audio From Cam Pos", true, true);
            unfreezeOnPlaybackChooser = Utils.SetupToggle(this, "Start Playback Unfreezes Motion", true, true);
            crosshairChooser = Utils.SetupToggle(this, "Crosshair", false, true);
            muteAudioChooser = Utils.SetupToggle(this, "Mute All Sound", true, true);
            VRAMWarningChooser = Utils.SetupToggle(this, "Ignore VRAM Warning", false, true);
            preserveTransparencyChooser = Utils.SetupToggle(this, "Preserve Transparency (PNG Only)", false, true);
            leaveEmptySphereVisible = Utils.SetupToggle(this, "Empty and Target Stay Visible", true, true);
            usePostProcessing = Utils.SetupToggle(this, "Use Post-Processing Effects", false, true);
            useCommandBuffers = Utils.SetupToggle(this, "Use Command Buffer Effects", false, true);

            JSONStorableFloat previewSizeChooser = Utils.SetupSliderFloat(this, "Preview Size (%)", previewSize * 100f, 1f, ((float)(Screen.height * (1 - previewPadding * 3f)) / (float)Screen.width) * 100f, true);

            Utils.SetupAction(this, "PlayPause", PlayPause);
            Utils.SetupButton(this, "Play / Pause Video", PlayPause, true);
            Utils.SetupAction(this, "PlayVideoAndAnimation", PlayVideoAndAnimation);
            Utils.SetupButton(this, "Play / Pause Video With Animation", PlayVideoAndAnimation, true);

            timeChooser = new JSONStorableFloat("Video Time", 0, 0, 10000, true, true);
            timeChooser.storeType = JSONStorableParam.StoreType.Full;
            timeSlider = CreateSlider(timeChooser, true);
            RegisterFloat(timeChooser);
            myMemoryInfo = new JSONStorableString("MemoryInfo", "");
            myDiskSpaceInfo = new JSONStorableString("DiskSpaceInfo", "");
            effectiveFrameRate = new JSONStorableString("FrameRateInfo", "");
            Utils.SetupInfoOneLine(this, myMemoryInfo, true);
            Utils.SetupInfoOneLine(this, myDiskSpaceInfo, true);
            Utils.SetupInfoOneLine(this, effectiveFrameRate, true);

            JSONStorableString filenameString = new JSONStorableString("baseFilename", "");
            UIDynamicLabelInput fileNameInput = Utils.SetupTextInput(this, "Filename", filenameString, true);

            JSONStorableStringChooser aspectRatioChooser = Utils.SetupStringChooser(this, "Aspect Ratio", ASPECTRATIO_NAMES, DEFAULT_ASPECTRATIO_IDX, true);
            JSONStorableStringChooser resolutionChooserOutput = Utils.SetupStringChooser(this, "Output Resolution", RESOLUTION_NAMES[DEFAULT_ASPECTRATIO_IDX], DEFAULT_RESOLUTION_IDX[DEFAULT_ASPECTRATIO_IDX], true);
            JSONStorableStringChooser formatChooser = Utils.SetupStringChooser(this, "Image Format", FORMAT_NAMES, DEFAULT_FORMAT, true);
            JSONStorableFloat jpegQualityChooser = Utils.SetupSliderInt(this, "JPEG Quality (%)", jpegQuality, 0, 100, true);
            audioSampleRangeChooser = Utils.SetupSliderInt(this, "Audio Sample Range (Prevent Popping)", (int)maxShort, 1, (int)maxShort, true);

            TextureSettings textureSettings = new TextureSettings();
            // force compression to resize texture dimensions into divisible by 4, otherwise junk data
            textureSettings.compress = true;
            textureSettings.linearColor = false;
            textureSettings.createMipMaps = false;
            textureSettings.anisoLevel = 0;
            textureSettings.bumpStrength = 0;
            textureSettings.filterMode = FilterMode.Bilinear;

            Utils.SetupAction(this, "ClearBackground", ClearBackground);
            Utils.SetupButton(this, "Clear Background", ClearBackground, true);

            previewBackgroundChooser = Utils.SetupTexture2DChooser(this, "Load Background Image", "", true, textureSettings, PreviewBackgroundLoaded, false);
            videoURL = new JSONStorableUrl("Video", string.Empty, (string url) => { LoadURLVideo(url); }, "mp4|asf|avi|dv|m4v|mov|mpg|mpeg|ogv|vp8|webm|wmv");
            RegisterUrl(videoURL);
            UIDynamicButton button = CreateButton("Load Background Video (No Autoplay)", true);
            videoURL.RegisterFileBrowseButton(button.button);
            JSONStorableStringChooser backgroundStereoLayoutChooser = Utils.SetupStringChooser(this, "BG Stereo Layout", STEREO_LAYOUTS, 0, true);
            loopChooser = Utils.SetupToggle(this, "Loop Video", true, true);

            unpauseVideoOnRenderChooser = Utils.SetupToggle(this, "Unpause Video On Render", true, true);

            renderBackgroundChooser = Utils.SetupToggle(this, "Render Background To Output", true, true);
            backgroundColor = Utils.SetupColor(this, "Background Color", Color.black, true);
            hideBackgroundColorOnPreviewOnly = Utils.SetupToggle(this, "Hide Background Color On Preview Only", false, true);

            Utils.SetupAction(this, "TakeSingleScreenshot", TakeSingleScreenshot);
            Utils.SetupAction(this, "StartPlaybackAndRecordVideo", StartPlaybackAndRecordVideo);
            Utils.SetupAction(this, "RecordVideo", RecordVideo);
            Utils.SetupAction(this, "SeekToBeginning", SeekToBeginning);
            Utils.SetupAction(this, "StopRecording", EndRender);

            Utils.SetupButton(this, "Take Single Screenshot (F9)", TakeSingleScreenshot, false);
            Utils.SetupButton(this, "Start Playback and Record Video (F10)", StartPlaybackAndRecordVideo, false);
            Utils.SetupButton(this, "Record Video (F11) (Escape To Cancel)", RecordVideo, false);
            Utils.SetupButton(this, "Seek To Beginning (F12)", SeekToBeginning, false);
            recordAudioChooser = Utils.SetupToggle(this, "Record Audio", true, false);
            pauseVideoChooser = Utils.SetupToggle(this, "Resume Last Recording", false, false);
            dazBvhScaleChooser = Utils.SetupToggle(this, "DAZ BVH Scale (cm units)", false, false);
            bvhStartsAtOrigin = Utils.SetupToggle(this, "BVH Starts At Origin", false, false);
            bvhUseUnorientedSkeleton = Utils.SetupToggle(this, "BVH Unoriented Rest Pose", false, false);

            JSONStorableStringChooser renderModeChooser = Utils.SetupStringChooser(this, "Render Mode", RENDERMODE_NAMES, DEFAULT_RENDERMODE_IDX, false);
            JSONStorableStringChooser stereoModeChooser = Utils.SetupStringChooser(this, "Stereo Mode", STEREOMODE_NAMES, STEREO_STATIC, false);
            JSONStorableFloat ipdChooser = Utils.SetupSliderFloatWithRange(this, "Interpupillary Distance (mm)", DEFAULT_IPD, 40, 90, false);
            JSONStorableFloat cubemapSideLengthChooser = Utils.SetupSliderInt(this, "Cubemap/Panoramic Side Resolution", cubemapSideLength, 128, 8192, false);
            JSONStorableStringChooser msaaChooser = Utils.SetupStringChooser(this, "VR & Flat MSAA", MSAA_NAMES, DEFAULT_MSAA_IDX, false);
            JSONStorableFloat secondsToRecordChooser = Utils.SetupSliderIntWithRange(this, "Seconds To Record", secondsToRecord, 1, 60, false);
            JSONStorableStringChooser frameRateChooser = Utils.SetupStringChooser(this, "Frame Rate", FRAMERATE_NAMES, DEFAULT_FRAMERATE_IDX, false);
            JSONStorableFloat framesToSkipChooser = Utils.SetupSliderIntWithRange(this, "Render Every nth Frame", 1, 1, 10, false);
            JSONStorableFloat lilyRenderOverlapChooser = Utils.SetupSliderFloat(this, "Smooth Stitching Overlap (%)", lilyRenderOverlap, 0f, 100f, false);

            List<string> sceneAtoms = new List<string>() { "None" };
            foreach (string id in SuperController.singleton.GetAtomUIDs())
            {
                if (id != null)
                    sceneAtoms.Add(id);
            }

            JSONStorableStringChooser focusObject = new JSONStorableStringChooser("Camera Target", sceneAtoms, "None", "Cam Target (Reload for New)");
            RegisterStringChooser(focusObject);
            UIDynamicPopup popup = CreateFilterablePopup(focusObject, false);
            JSONStorableFloat flatHorizontalFovChooser = Utils.SetupSliderFloat(this, "Flat Horizontal FOV", flatHorizontalFov, 0.1f, 179.9f, false);
            JSONStorableFloat flatSupersamplingChooser = Utils.SetupSliderInt(this, "Flat Supersampling Multiplier", flatSupersampling, 1, 8, false);
            JSONStorableStringChooser kernelModeChooser = Utils.SetupStringChooser(this, "Flat Kernel Mode", KERNEL_NAMES, myKernelMode, false);

            hideSizeChooser = Utils.SetupSliderFloat(this, "Hide Vertical Extremes Size (Degrees)", 18, 0, 100, false);
            eyePivotDistanceChooser = Utils.SetupSliderFloatWithRange(this, "Eye Pivot Distance (mm) (Stereo)", 0, 0, 200, false);
            frontFovChooser = Utils.SetupSliderFloat(this, "Front FOV (Triangle Stereo)", 120, 90, 170, false);
            hideSeamsSizeChooser = Utils.SetupSliderFloatWithRange(this, "Hide Seams Size (Triangle/Square)", 0, 0, 50, false);
            hideSeamsParallaxChooser = Utils.SetupSliderFloatWithRange(this, "Hide Seams Parallax (Triangle/Square)", 0, 0, 10, false);
            Utils.SetupAction(this, "ClearSeamTexture", ClearSeamTexture);
            Utils.SetupButton(this, "Clear Seam Texture", ClearSeamTexture, false);
            seamTextureUrl = Utils.SetupTexture2DChooser(this, "Load Seam Texture", "", false, textureSettings, SeamTextureLoaded, false);
            seamTextureTintChooser = Utils.SetupColor(this, "Seam Tint", Color.black, false);

            previewAudioFromCamPos.setCallbackFunction += (bool b) =>
            {
                GameObject currentListener = GameObject.FindObjectOfType<AudioListener>()?.gameObject;
                GameObject newListener = null;

                if (b)
                    newListener = containingAtom.mainController.gameObject;
                else
                    newListener = originalAudioListener;

                if (newListener == null)
                    return;

                if (currentListener != newListener)
                {
                    if (currentListener != null)
                    {
                        currentListener.GetComponent<AudioListener>().enabled = false;
                        Destroy(currentListener.GetComponent<AudioListener>());
                    }
                    newListener.AddComponent<AudioListener>();
                    newListener.GetComponent<AudioListener>().velocityUpdateMode = originalVelocityUpdateMode;
                }
            };

            focusObject.setCallbackFunction += (string v) =>
            {
                Atom atom = SuperController.singleton.GetAtomByUid(v);
                if (atom != null && atom != containingAtom)
                {
                    if (atom.type == "Person")
                        cameraTarget = atom.GetStorableByID("headControl").transform;
                    else
                        cameraTarget = atom.GetStorableByID("control").transform;

                    Transform t;
                    if ((t = cameraTarget.parent) != null && t.gameObject.name == "reParentObject")
                    {
                        if ((t = t.Find("object")) != null)
                        {
                            if ((t = t.Find("rescaleObject")) != null)
                            {
                                if ((t = t.Find("SphereRender")) != null)
                                {
                                    if (t.gameObject.GetComponent<MeshRenderer>() != null)
                                    {
                                        int layer = LayerMask.NameToLayer("UI");
                                        if (targetSphereObject != null)
                                        {
                                            targetSphereObject.layer ^= layer;
                                            SyncSelectionVisibility();
                                        }

                                        targetSphereObject = t.gameObject;

                                        if ((t.gameObject.layer & layer) != layer)
                                        {
                                            targetSphereObject.layer |= layer;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (targetSphereObject != null)
                    {
                        SyncSelectionVisibility();
                        targetSphereObject.layer ^= LayerMask.NameToLayer("UI");
                    }

                    targetSphereObject = null;
                    cameraTarget = null;
                }
            };

            leaveEmptySphereVisible.setCallbackFunction += (bool b) =>
            {
                SyncSelectionVisibility();
            };

            backgroundStereoLayoutChooser.setCallbackFunction += (string v) =>
            {
                backgroundStereoLayout = STEREO_LAYOUTS.FindIndex((string entry) => { return entry == v; });
                if (backgroundStereoLayout < 0)
                    backgroundStereoLayout = BG_MONO;
            };

            stereoModeChooser.setCallbackFunction += (string v) =>
            {
                stereoMode = STEREOMODE_NAMES.FindIndex((string entry) => { return entry == v; });
                if (stereoMode < 0)
                    stereoMode = STEREO_STATIC;

                oldOverlap = -1f;
                myNeedSetup = true;
            };

            hideBackgroundColorOnPreviewOnly.setCallbackFunction += (bool v) =>
            {
                if (v)
                {
                    previewCamera.backgroundColor = Color.clear;
                }
                else if (!(preserveTransparencyChooser.val && myFileFormat == FORMAT_PNG))
                {
                    Color color = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);
                    previewCamera.backgroundColor = color;
                }
                oldOverlap = -1;
                myNeedSetup = true;
            };

            backgroundColor.setCallbackFunction += (float h, float s, float v) =>
            {
                if (!(preserveTransparencyChooser.val && myFileFormat == FORMAT_PNG) && !hideBackgroundColorOnPreviewOnly.val)
                    previewCamera.backgroundColor = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);

                oldOverlap = -1;
                myNeedSetup = true;
            };

            preserveTransparencyChooser.setCallbackFunction += (bool v) =>
            {
                if (v && myFileFormat == FORMAT_PNG)
                {
                    previewCamera.backgroundColor = Color.clear;
                }
                else if (!hideBackgroundColorOnPreviewOnly.val)
                {
                    Color color = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);
                    previewCamera.backgroundColor = color;
                }

                oldOverlap = -1;
                myNeedSetup = true;
            };

            frontFovChooser.setCallbackFunction += (float v) =>
            {
                oldOverlap = -1;
                myNeedSetup = true;
            };


            eyePivotDistanceChooser.setCallbackFunction += (float v) =>
            {
                oldOverlap = -1;
                myNeedSetup = true;
            };


            timeChooser.setCallbackFunction += (float v) =>
            {
                if (insideUpdate)
                    return;
                else if (videoPlayer != null)
                    videoPlayer.time = v;
            };

            loopChooser.setCallbackFunction += (bool v) =>
            {
                if (videoPlayer != null)
                    videoPlayer.isLooping = v;
            };

            lilyRenderOverlapChooser.setCallbackFunction += (float v) =>
            {
                lilyRenderOverlap = v / 100f;

                myNeedSetup = true;
            };

            renderBackgroundChooser.setCallbackFunction += (bool b) =>
            {
                myNeedSetup = true;
            };

            previewChooser.setCallbackFunction += (bool v) =>
            {
            };

            previewSizeChooser.setCallbackFunction += (float v) =>
            {
                previewSize = v / 100f;
                myNeedSetup = true;
            };

            kernelModeChooser.setCallbackFunction += (string v) =>
            {
                myKernelMode = kernelModeChooser.choices.FindIndex((string entry) => { return entry == v; });
            };

            flatSupersamplingChooser.setCallbackFunction += (float v) =>
            {
                if (v != 1 && myConvolutionMaterial == null)
                {
                    flatSupersampling = 1;
                    flatSupersamplingChooser.val = 1;
                }
                else
                    flatSupersampling = (int)v;

                myNeedSetup = true;
            };

            flatHorizontalFovChooser.setCallbackFunction += (float v) =>
            {
                flatHorizontalFov = v;
                myNeedSetup = true;
            };

            jpegQualityChooser.setCallbackFunction += (float v) =>
            {
                jpegQuality = (int)v;

                myNeedSetup = true;
            };

            filenameString.setCallbackFunction += (string s) =>
            {
                baseFilename = s;
            };

            cubemapSideLengthChooser.setCallbackFunction += (float v) =>
            {
                selectedCubemapSideLength = (int)v;

                myNeedSetup = true;
            };

            framesToSkipChooser.setCallbackFunction += (float v) =>
            {
                framesToSkip = (int)v - 1;

                myNeedSetup = true;
            };

            frameRateChooser.setCallbackFunction += (string v) =>
            {
                frameRateIdx = frameRateChooser.choices.FindIndex((string entry) => { return entry == v; });
                if (frameRateIdx < 0)
                    frameRateIdx = DEFAULT_FRAMERATE_IDX;

                frameRateInt = int.Parse(v);

                myNeedSetup = true;
            };
            secondsToRecordChooser.setCallbackFunction += (float v) =>
            {
                secondsToRecord = (int)v;
                myNeedSetup = true;
            };

            ipdChooser.setCallbackFunction += (float v) =>
            {
                ipd = v;
                myNeedSetup = true;
            };

            renderModeChooser.setCallbackFunction += (string v) =>
            {
                renderModeIdx = RENDERMODE_NAMES.FindIndex((string entry) => { return entry == v; });
                if (renderModeIdx < 0)
                    renderModeIdx = DEFAULT_RENDERMODE_IDX;

                bBvhRender = renderModeIdx == 5;
                bStereoRender = renderModeIdx == 1 || renderModeIdx == 2;
                b180Degrees = renderModeIdx == 1 || renderModeIdx == 3;
                bFlatRender = renderModeIdx == 0 || renderModeIdx == 5;

                oldOverlap = -1f;
                myNeedSetup = true;
            };

            aspectRatioChooser.setCallbackFunction += (string v) =>
            {
                int aspectRatioIdx = ASPECTRATIO_NAMES.FindIndex((string entry) => { return entry == v; });
                if (aspectRatioIdx < 0)
                    aspectRatioIdx = DEFAULT_ASPECTRATIO_IDX;
                if (aspectRatioIdx != myAspectRatioIdx)
                {
                    myAspectRatioIdx = aspectRatioIdx;
                    resolutionChooserOutput.choices = RESOLUTION_NAMES[myAspectRatioIdx];
                    resolutionChooserOutput.valNoCallback = RESOLUTION_NAMES[myAspectRatioIdx][DEFAULT_RESOLUTION_IDX[myAspectRatioIdx]];
                    resolutionChooserOutput.setCallbackFunction(resolutionChooserOutput.val);
                }
                myNeedSetup = true;
            };
            resolutionChooserOutput.setCallbackFunction += (string v) =>
            {
                myResolutionIdx = resolutionChooserOutput.choices.FindIndex((string entry) => { return entry == v; });
                if (myResolutionIdx < 0)
                    myResolutionIdx = DEFAULT_RESOLUTION_IDX[myAspectRatioIdx];
                myNeedSetup = true;
            };

            msaaChooser.setCallbackFunction += (string v) =>
            {
                int idx = MSAA_NAMES.FindIndex((string entry) => { return entry == v; });
                if (idx < 0 || idx >= MSAA_VALUES.Count)
                    idx = DEFAULT_MSAA_IDX;
                myMsaaLevel = MSAA_VALUES[idx];
                myNeedSetup = true;
            };

            formatChooser.setCallbackFunction += (string v) =>
            {
                myFileFormat = FORMAT_NAMES.FindIndex((string entry) => { return entry == v; });
                if (myFileFormat < 0 || myFileFormat >= FORMAT_NAMES.Count)
                    myFileFormat = DEFAULT_FORMAT;
                myNeedSetup = true;
            };
        }

        void SyncSelectionVisibility()
        {
            var selectedController = SuperController.singleton.GetSelectedController();

            if (cameraTarget != null && targetSphereObject != null)
            {
                if (selectedController?.gameObject != cameraTarget.gameObject)
                    targetSphereObject.GetComponent<MeshRenderer>().enabled = false;
                else
                    targetSphereObject.GetComponent<MeshRenderer>().enabled = true;
            }
            if (sphereObject != null)
            {
                if (selectedController?.gameObject != myContainingAtom?.mainController?.gameObject)
                    sphereObject.GetComponent<MeshRenderer>().enabled = false;
                else
                    sphereObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        void SeamTextureLoaded(Texture2D texture)
        {
            seamTexture = texture;
            seamTextureUrl.val = ""; // VaM won't load the same image twice in a row?
        }

        void ClearSeamTexture()
        {
            Destroy(seamTexture);
            seamTexture = null;
        }

        string GetVAMPath()
        {
            string dataPath = Application.dataPath.TrimEnd('/', '\\');
            int forwardSlash = dataPath.LastIndexOf('/');
            int backwardSlash = dataPath.LastIndexOf('\\');
            if ((forwardSlash != -1) && (forwardSlash > backwardSlash))
                dataPath = dataPath.Substring(0, forwardSlash);
            else if (forwardSlash != -1)
                dataPath = dataPath.Substring(0, backwardSlash);
            return dataPath;
        }

        void LoadURLVideo(string url)
        {
            try
            {
                ClearBackground();
                if (go == null)
                    go = new GameObject();
                if (go.GetComponent<VideoPlayer>() == null)
                    go.AddComponent<VideoPlayer>();

                if (go.GetComponent<VideoPlayer>() != null)
                {
                    videoPlayer = go.GetComponent<VideoPlayer>();
                    videoPlayer.playOnAwake = false;
                    videoPlayer.url = url.Contains(":/") ? url : (GetVAMPath() + "/" + url);
                    videoPlayer.isLooping = loopChooser.val;
                    videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    videoPlayer.errorReceived += VideoErrorEvent;
                    videoPlayer.prepareCompleted += VideoPreparedEvent;
                    videoPlayer.Prepare();
                }
            }
            catch (Exception e)
            {
                SuperController.LogError(e.ToString());
            }
        }

        void PlayPause()
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }

        void PlayVideoAndAnimation()
        {
            PlayPause();
            if (videoPlayer.isPlaying)
            {
                SuperController.singleton.motionAnimationMaster.StartPlayback();
                if (unfreezeOnPlaybackChooser.val)
                {
                    if (SuperController.singleton.freezeAnimation)
                    {
                        SuperController.singleton.SetFreezeAnimation(false);
                        unfrozeOnStart = true;
                    }
                    else
                        unfrozeOnStart = false;
                }
            }
            else
            {
                if (unfreezeOnPlaybackChooser.val && unfrozeOnStart)
                {
                    SuperController.singleton.SetFreezeAnimation(true);
                    unfrozeOnStart = false;
                }
                SuperController.singleton.motionAnimationMaster.StopPlayback();
            }

        }

        void VideoErrorEvent(UnityEngine.Video.VideoPlayer v, string e)
        {
            SuperController.LogMessage("Unity video player error: " + e);
            videoPlayer = null;
        }

        void VideoPreparedEvent(UnityEngine.Video.VideoPlayer videoPlayer)
        {
            if (previewBackground != null)
                Destroy(previewBackground);
            videoPlayer.renderMode = VideoRenderMode.APIOnly;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += VideoFrameReady;
            videoPlayer.Play();
            videoNotPausedYet = true;
            float videoLength = (float)videoPlayer.frameCount / (float)videoPlayer.frameRate;

            timeSlider.slider.value = 0f;
            timeSlider.slider.maxValue = videoLength;
            myNeedSetup = true;
        }

        void ClearBackground()
        {
            if (previewBackground != null)
            {
                Destroy(previewBackground);
                previewBackground = null;
            }
            if (videoPlayer != null)
            {
                videoPlayer = null;
            }
        }

        void PreviewBackgroundLoaded(Texture2D texture)
        {
            ClearBackground();
            previewBackground = texture;
            myNeedSetup = true;
            previewBackgroundChooser.val = ""; // VaM won't load the same image twice in a row?
        }

        private void OnDestroy()
        {
            try
            {
                SyncSelectionVisibility();

                Camera.onPreRender -= OnPreRenderCallback;
                // Camera.onPostRender -= OnPostRenderCallback;

                if (sphereLayerChanged)
                    sphereObject.layer ^= LayerMask.NameToLayer("UI");

                if (targetSphereObject)
                    targetSphereObject.layer ^= LayerMask.NameToLayer("UI");

                GameObject currentAudioListener = GameObject.FindObjectOfType<AudioListener>()?.gameObject;
                if (currentAudioListener != originalAudioListener && currentAudioListener != null)
                {
                    currentAudioListener.GetComponent<AudioListener>().enabled = false;
                    Destroy(currentAudioListener.GetComponent<AudioListener>());
                }

                if (originalAudioListener != null)
                {
                    originalAudioListener.AddComponent<AudioListener>();
                    originalAudioListener.GetComponent<AudioListener>().velocityUpdateMode = originalVelocityUpdateMode;
                }

                Destroy(previewTex);
                Destroy(previewCamera);
                Destroy(blackTex);
                Destroy(semiTex);
                if (previewBackground != null)
                    Destroy(previewBackground);

                if (renderCamParentObj != null)
                    EndVRPreview();

                if (go != null)
                    Destroy(go);

                Utils.OnDestroyUI();

                if (myLilyRenderBundleURL != null)
                {
                    AssetLoader.DoneWithAssetBundleFromFile(myLilyRenderBundleURL);
                    myLilyRenderBundleURL = null;
                }
                if (myDownsampleBundleURL != null)
                {
                    AssetLoader.DoneWithAssetBundleFromFile(myDownsampleBundleURL);
                    myDownsampleBundleURL = null;
                }
            }
            catch (Exception e)
            {
                SuperController.LogError(e.ToString());
            }

        }

        private void OnDownsampleBundleLoaded(Request aRequest)
        {
            myConvolutionMaterial = aRequest.assetBundle?.LoadAsset<Material>("Assets/SuperResolution/Convolution.mat");

            if (myConvolutionMaterial == null)
                SuperController.LogError("VRRenderer: Could not load flat supersampling shader, probably because SuperShot is loaded. Supersampling is unavailable but this does not affect any other functionality.");
            myNeedSetup = true;
        }

        private void OnLilyRenderBundleLoaded(Request aRequest)
        {
            _equirectMat = aRequest.assetBundle?.LoadAsset<Material>("Assets/LilyRender.mat");
            if (_equirectMat == null)
                SuperController.LogError("VRRenderer: Could not load LilyRender shader!");
            _equirectMatAlpha = aRequest.assetBundle?.LoadAsset<Material>("Assets/LilyRenderAlpha.mat");
            if (_equirectMatAlpha == null)
                SuperController.LogError("VRRenderer: Could not load LilyRenderAlpha shader!");
            _equirectMatRotate = aRequest.assetBundle?.LoadAsset<Material>("Assets/LilyRenderRotate.mat");
            if (_equirectMatRotate == null)
                SuperController.LogError("VRRenderer: Could not load LilyRenderRotate shader!");
            _equirectMatRotateTriangle = aRequest.assetBundle?.LoadAsset<Material>("Assets/LilyRenderRotateTriangle.mat");
            if (_equirectMatRotateTriangle == null)
                SuperController.LogError("VRRenderer: Could not load LilyRenderRotateTriangle shader!");
            sliceEquirectMat = aRequest.assetBundle?.LoadAsset<Material>("Assets/PixelSliceEquirect.mat");
            if (sliceEquirectMat == null)
                SuperController.LogError("VRRenderer: Could not load PixelSliceEquirect shader!");
            alphaDiffMat = aRequest.assetBundle?.LoadAsset<Material>("Assets/AlphaFromDifference.mat");
            if (alphaDiffMat == null)
                SuperController.LogError("VRRenderer: Could not load AlphaFromDifference shader!");

            oldPreviewSize = -1f;
            myNeedSetup = true;
        }

        private float GetMemoryEstimate()
        {
            if (bBvhRender)
                return 0f;

            float bytesToGB = 1f / (1024f * 1024f * 1024f);
            long msaaFactor = myMsaaLevel > 1 ? (myMsaaLevel + 1) : 1;
            int alphaDifferenceFactor = (myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null)) ? 2 : 0;

            if (bFlatRender)
            {
                long render = 8L * (long)msaaFactor * (long)finalResolution.x * (long)finalResolution.y * (long)flatSupersampling * (long)flatSupersampling;
                long pass = 4L * (long)finalResolution.x * (long)finalResolution.y * (long)flatSupersampling;
                long output = 3L * (long)finalResolution.x * (long)finalResolution.y;

                if (flatSupersampling > 1)
                    return 1f * ((2f + alphaDifferenceFactor) * render + 2f * pass + output) * bytesToGB;
                else
                    return 1f * ((2f + alphaDifferenceFactor) * render + output) * bytesToGB;
            }
            else
            {
                long cubemapFace = 8L * (long)msaaFactor * (long)cubemapSideLength * (long)cubemapSideLength;
                long equirect = 8L * ((long)finalResolution.x / 2L) * (long)finalResolution.y;
                long output = 3L * (long)finalResolution.x * (long)finalResolution.y;
                long slice = 8L * 1L * (long)msaaFactor * (long)cubemapSideLength;

                long camera = cubemapFace;

                if (bStereoRender && stereoMode == STEREO_TRIANGLE)
                    return ((9f + alphaDifferenceFactor) * cubemapFace + camera + 2f * equirect + output) * bytesToGB;
                else if (bStereoRender && stereoMode == STEREO_SQUARE)
                    return ((6f + alphaDifferenceFactor) * cubemapFace + camera + 2f * equirect + output) * bytesToGB;
                else if (bStereoRender && stereoMode == STEREO_PANORAMIC)
                    return ((3f + alphaDifferenceFactor) * slice + 3f * equirect + output) * bytesToGB;
                else if (bStereoRender && stereoMode == STEREO_STATIC)
                    return ((6f + alphaDifferenceFactor) * cubemapFace + camera + 2f * equirect + output) * bytesToGB;
                else if (!bStereoRender)
                    return ((6f + alphaDifferenceFactor) * cubemapFace + camera + equirect + output) * bytesToGB;
                else return -1f;
            }
        }

        private float GetDiskSpaceEstimate()
        {
            if (bBvhRender)
                return 0f;

            float bytesToGB = 1f / (1024f * 1024f * 1024f);
            long frame = (long)finalResolution.x * (long)finalResolution.y;
            int frames = (int)(effectiveFrameRateInt * secondsToRecord);
            float compression = myFileFormat == FORMAT_JPG ? 0.4f : 0.8f;
            if (myFileFormat == FORMAT_JPG)
            {
                if (jpegQuality < 80)
                    compression *= 0.15f;
                else if (jpegQuality < 85)
                    compression *= 0.33f;
                else if (jpegQuality < 90)
                    compression *= 0.4f;
                else if (jpegQuality < 95)
                    compression *= 0.5f;
                else if (jpegQuality <= 98)
                    compression *= 0.70f;
                else if (jpegQuality == 99)
                    compression *= 0.85f;
            }

            return frame * frames * compression * bytesToGB;
        }

        bool requestingSetup;
        IEnumerator RequestSetupDelayed(float delay)
        {
            if (requestingSetup)
                yield break;
            requestingSetup = true;
            yield return new WaitForSeconds(delay);
            if (requestingSetup)
            {
                requestingSetup = false;
                myNeedSetup = true;
            }
        }

        private void Setup()
        {
            cubemapSideLength = (int)(selectedCubemapSideLength * (1f + lilyRenderOverlap * 2));
            finalResolution = RESOLUTION_VALUES[myAspectRatioIdx][myResolutionIdx];

            float aspect = (float)finalResolution.x / (float)finalResolution.y;
            flatVerticalFov = Mathf.Rad2Deg * 2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * flatHorizontalFov * 0.5f) / aspect);

            previewResolution = (int)((int)(Screen.width * previewSize) / 2) * 2 + 1;
            int previewHeight = 0;
            if (b180Degrees)
                previewHeight = previewResolution;
            else
                previewHeight = (int)(previewResolution / 4) * 2 + 1;
            if (bFlatRender)
                previewHeight = (int)((int)(previewResolution / aspect) / 2) * 2 + 1;

            if (previewTex == null || previewResolution != previewTex.width || previewHeight != previewTex.height || oldFlatMsaaLevel != myMsaaLevel)
            {
                Destroy(previewTex);
                Destroy(previewTexBlack);
                Destroy(previewTexWhite);
                previewTexBlack = GetRenderTexture(previewResolution, previewHeight, myMsaaLevel, true);
                previewTexWhite = GetRenderTexture(previewResolution, previewHeight, myMsaaLevel, true);
                previewTex = GetRenderTexture(previewResolution, previewHeight, myMsaaLevel, true);
                oldFlatMsaaLevel = myMsaaLevel;
            }

            previewCamera.targetTexture = previewTex;
            previewCamera.fieldOfView = bFlatRender ? flatVerticalFov : flatHorizontalFov;

            // changed cubemap res or preview size or msaa or resolution
            if (!bFlatRender && previewChooser.val && _equirectMat != null && (oldPreviewSize != previewSize || oldSelectedCubemapSideLength != selectedCubemapSideLength || oldMsaaLevel != myMsaaLevel || oldRenderMode != renderModeIdx || oldResolution != myResolutionIdx || oldOverlap != lilyRenderOverlap))
            {
                if (Time.realtimeSinceStartup - lastResize > 1f / previewResizePerSecond)
                {
                    requestingSetup = false;
                    oldPreviewSize = previewSize;
                    oldSelectedCubemapSideLength = selectedCubemapSideLength;
                    oldMsaaLevel = myMsaaLevel;
                    oldRenderMode = renderModeIdx;
                    oldResolution = myResolutionIdx;
                    oldOverlap = lilyRenderOverlap;
                    if (renderCamParentObj != null)
                        EndVRPreview();
                    BeginVRPreview();
                    if (lastResize + 1f / previewResizePerSecond > Time.realtimeSinceStartup)
                        lastResize += 1f / previewResizePerSecond;
                    else
                        lastResize = Time.realtimeSinceStartup;
                }
                else if (!requestingSetup)
                {
                    StartCoroutine("RequestSetupDelayed", 1f / previewResizePerSecond);
                }
            }

            if (!bBvhRender)
                effectiveFrameRateInt = (int)((float)frameRateInt / (float)(framesToSkip > 0f ? framesToSkip + 1 : 1f));
            else
                effectiveFrameRateInt = frameRateInt;

            effectiveFrameRate.val = "<b>Effective Frame Rate: " + effectiveFrameRateInt + " FPS</b>";

            float diskSpaceEstimate = GetDiskSpaceEstimate();
            if (myDiskSpaceEstimate != diskSpaceEstimate)
            {
                myDiskSpaceEstimate = diskSpaceEstimate;
                myDiskSpaceInfo.val = "<b>Video Frames Size: ~" + myDiskSpaceEstimate.ToString("0.0") + " GB</b>";
            }

            float memoryEstimate = GetMemoryEstimate();
            if (myMemoryEstimate != memoryEstimate)
            {
                myMemoryEstimate = memoryEstimate;
                string color = "";
                if (myMemoryEstimate * 1024 > SystemInfo.graphicsMemorySize - 3072)
                    color = "FF0000";
                else if (myMemoryEstimate * 1024 > SystemInfo.graphicsMemorySize - 4096)
                    color = "FF8800";
                myMemoryInfo.val = color != "" ? "<color=#" + color + ">" : "";
                myMemoryInfo.val += "<b>Estimated VRAM Usage: ~" + myMemoryEstimate.ToString("0.0") + " GB</b>";
                myMemoryInfo.val += color != "" ? "</color>" : "";
            }

            if (myConvolutionMaterial != null)
                SetupDownsampleMaterial();

            myNeedSetup = false;
        }

        void OnGUI()
        {
            if (!previewStaysOpenChooser.val && !bRendering && containingAtom.mainController != SuperController.singleton.GetSelectedController())
                return;
            if (previewChooser.val && Event.current.type.Equals(EventType.Repaint) && previewTex != null)
            {
                int width = previewTex.width;
                int height = previewTex.height;
                int pad = (int)(Screen.width * previewPadding);
                Rect previewRect;
                float barHeight = 18;

                if (bRendering && bRecordVideo)
                {
                    previewRect = new Rect(Screen.width - width - pad, Screen.height - height - pad - barHeight, width, height);
                }
                else
                {
                    previewRect = new Rect(Screen.width - width - pad, Screen.height - height - pad, width, height);
                }

                GL.sRGBWrite = true;

                /*
                if (renderFaces != null && bStereoRender)
                {
                    if (stereoMode == STEREO_TRIANGLE)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            Rect a = new Rect(Screen.width - 600 + (i % 3) * 200, (i / 3) * 200, 200, 200);
                            Graphics.DrawTexture(a, renderFaces[i]);
                        }
                    }
                    else if (stereoMode == STEREO_SQUARE)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            Rect a = new Rect(Screen.width - 1200 + (i % 6) * 200, (i / 6) * 200, 200, 200);
                            Graphics.DrawTexture(a, renderFaces[i]);
                        }
                    }
                }
                else if(renderFaces != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Rect a = new Rect(Screen.width - 600 + (i % 3) * 200, (i / 3) * 200, 200, 200);
                        Graphics.DrawTexture(a, renderFaces[i]);
                    }
                }
                */

                Texture backgroundTexture;
                if (previewBackground != null)
                    backgroundTexture = previewBackground;
                else if (videoPlayer != null && videoPlayer.texture != null)
                    backgroundTexture = videoPlayer.texture;
                else
                    backgroundTexture = null;

                if (backgroundTexture != null)
                {
                    Rect sourceRect = default(Rect);
                    if (backgroundStereoLayout == BG_MONO)
                        sourceRect = new Rect(0, 0, 1f, 1f);
                    else if (backgroundStereoLayout == BG_LEFTRIGHT)
                        sourceRect = new Rect(0, 0, 0.5f, 1f);
                    else if (backgroundStereoLayout == BG_RIGHTLEFT)
                        sourceRect = new Rect(0.5f, 0, 0.5f, 1f);
                    else if (backgroundStereoLayout == BG_DOWNUP)
                        sourceRect = new Rect(0, 0f, 1f, 0.5f);
                    else if (backgroundStereoLayout == BG_UPDOWN)
                        sourceRect = new Rect(0, 0.5f, 1f, 0.5f);
                    Graphics.DrawTexture(previewRect, backgroundTexture, sourceRect, 0, 0, 0, 0, new Color(0.5f, 0.5f, 0.5f, 0.5f));
                }
                if (bRendering)
                {
                    if (bFlatRender)
                        Graphics.DrawTexture(previewRect, finalOutputTexture);
                    else
                        Graphics.DrawTexture(previewRect, equirectL);
                }
                else
                {
                    if (!bFlatRender && equirectL != null)
                    {
                        Graphics.DrawTexture(previewRect, equirectL);
                    }
                    else if (previewTex != null)
                        Graphics.DrawTexture(previewRect, previewTex);
                }

                if (!bFlatRender && bStereoRender && stereoMode == STEREO_PANORAMIC)
                {
                    float portion = hideSizeChooser.val / 180f;
                    Rect topHider = new Rect(previewRect.min.x, previewRect.min.y, previewRect.width, previewRect.height * portion);
                    Rect botHider = new Rect(previewRect.min.x, previewRect.max.y - previewRect.height * portion, previewRect.width, previewRect.height * portion);
                    Graphics.DrawTexture(topHider, blackTex);
                    Graphics.DrawTexture(botHider, blackTex);
                }

                int borderWidth = 1;

                Rect topBorder = new Rect(previewRect.min.x - borderWidth, previewRect.min.y - borderWidth, previewRect.width + 2 * borderWidth, borderWidth);
                Rect bottomBorder = new Rect(previewRect.min.x - borderWidth, previewRect.max.y, previewRect.width + 2 * borderWidth, borderWidth);
                Rect leftBorder = new Rect(previewRect.min.x - borderWidth, previewRect.min.y - borderWidth, borderWidth, previewRect.height + 2 * borderWidth);
                Rect rightBorder = new Rect(previewRect.max.x, previewRect.min.y - borderWidth, borderWidth, previewRect.height + 2 * borderWidth);

                Graphics.DrawTexture(topBorder, Texture2D.whiteTexture);
                Graphics.DrawTexture(bottomBorder, Texture2D.whiteTexture);
                Graphics.DrawTexture(leftBorder, Texture2D.whiteTexture);
                Graphics.DrawTexture(rightBorder, Texture2D.whiteTexture);

                if (crosshairChooser.val)
                {
                    Texture2D backTex = semiTex;
                    Texture2D outerTex = Texture2D.whiteTexture;
                    Texture2D innerTex = blackTex;
                    Texture2D centerTex = Texture2D.whiteTexture;

                    int lineLength = (int)(width * 0.2f);
                    int offsetW = width - 1;
                    int offsetH = height - 1;

                    Rect backHLine = new Rect(previewRect.min.x, previewRect.min.y + offsetH / 2f, offsetW, 1);
                    Rect backVLine = new Rect(previewRect.min.x + offsetW / 2f, previewRect.min.y, 1, offsetH);

                    int outerLength = lineLength / 6;
                    Rect lineLeft = new Rect(previewRect.min.x, previewRect.min.y + offsetH / 2, outerLength, 1);
                    Rect lineRight = new Rect(previewRect.max.x - outerLength, previewRect.min.y + offsetH / 2, outerLength, 1);
                    Rect lineTop = new Rect(previewRect.min.x + offsetW / 2, previewRect.min.y, 1, outerLength);
                    Rect lineBottom = new Rect(previewRect.min.x + offsetW / 2, previewRect.max.y - outerLength, 1, outerLength);

                    Rect horizontalLine = new Rect(previewRect.min.x + offsetW / 2 - lineLength / 2, previewRect.min.y + offsetH / 2, lineLength, 1);
                    Rect verticalLine = new Rect(previewRect.min.x + offsetW / 2, previewRect.min.y + offsetH / 2 - lineLength / 2, 1, lineLength);

                    Rect horizontalLineSmall = new Rect(previewRect.min.x + offsetW / 2 - (3 * lineLength) / 10, previewRect.min.y + offsetH / 2, 2 * (3 * lineLength) / 10, 1);
                    Rect verticalLineSmall = new Rect(previewRect.min.x + offsetW / 2, previewRect.min.y + offsetH / 2 - (3 * lineLength) / 10, 1, 2 * (3 * lineLength) / 10);

                    Rect horizontalLineSmaller = new Rect(previewRect.min.x + offsetW / 2 - lineLength / 8, previewRect.min.y + offsetH / 2, lineLength / 4, 1);
                    Rect verticalLineSmaller = new Rect(previewRect.min.x + offsetW / 2, previewRect.min.y + offsetH / 2 - lineLength / 8, 1, lineLength / 4);

                    Rect dot = new Rect(previewRect.min.x + offsetW / 2 - 1, previewRect.min.y + offsetH / 2 - 1, 3, 3);

                    Graphics.DrawTexture(backHLine, backTex);
                    Graphics.DrawTexture(backVLine, backTex);
                    Graphics.DrawTexture(lineLeft, centerTex);
                    Graphics.DrawTexture(lineRight, centerTex);
                    Graphics.DrawTexture(lineTop, centerTex);
                    Graphics.DrawTexture(lineBottom, centerTex);
                    Graphics.DrawTexture(horizontalLine, outerTex);
                    Graphics.DrawTexture(verticalLine, outerTex);
                    Graphics.DrawTexture(horizontalLineSmall, innerTex);
                    Graphics.DrawTexture(verticalLineSmall, innerTex);
                    Graphics.DrawTexture(horizontalLineSmaller, centerTex);
                    Graphics.DrawTexture(verticalLineSmaller, centerTex);
                    Graphics.DrawTexture(dot, innerTex);
                }

                if (bRendering && bRecordVideo)
                {
                    float dist = 0.2f;
                    float progress = ((float)frameCounter / (float)effectiveFrameRateInt) / (float)secondsToRecord;

                    float minWidth = 350;
                    float barWidth = Mathf.Max(previewRect.max.x - previewRect.min.x, minWidth);

                    float leftSide = previewRect.max.x - barWidth;
                    Rect background = new Rect(leftSide, previewRect.max.y, barWidth, barHeight);
                    Rect foreground = new Rect(leftSide, previewRect.max.y, barWidth * progress, barHeight);

                    topBorder = new Rect(background.min.x - borderWidth, background.min.y - borderWidth, background.width + 2 * borderWidth, borderWidth);
                    bottomBorder = new Rect(background.min.x - borderWidth, background.max.y, background.width + 2 * borderWidth, borderWidth);
                    leftBorder = new Rect(background.min.x - borderWidth, background.min.y - borderWidth, borderWidth, background.height + 2 * borderWidth);
                    rightBorder = new Rect(background.max.x, background.min.y - borderWidth, borderWidth, background.height + 2 * borderWidth);

                    Graphics.DrawTexture(background, blackTex);
                    Graphics.DrawTexture(topBorder, Texture2D.whiteTexture);
                    Graphics.DrawTexture(bottomBorder, Texture2D.whiteTexture);
                    Graphics.DrawTexture(leftBorder, Texture2D.whiteTexture);
                    Graphics.DrawTexture(rightBorder, Texture2D.whiteTexture);
                    Graphics.DrawTexture(foreground, Texture2D.whiteTexture);

                    int labelWidth = 320;
                    int padding = 2;
                    Rect textLabel = new Rect(background.min.x + (background.max.x - background.min.x) / 2 - labelWidth / 2, background.min.y, labelWidth, (background.max.y - background.min.y));
                    Rect labelBackground = new Rect(background.min.x + (background.max.x - background.min.x) / 2 - (labelWidth / 2) + padding, background.min.y + padding, labelWidth - 2 * padding, (background.max.y - background.min.y) - 2 * padding);

                    if (timestamps.Count > 1)
                    {
                        int index = 0;
                        for (int i = timestamps.Count - 1; i >= 0; i--)
                        {
                            if (Time.realtimeSinceStartup - timestamps[i] > timeStampCollectionDuration)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index != 0)
                            timestamps = timestamps.GetRange(index, timestamps.Count - index);

                        TextAnchor oldAlignment = GUI.skin.label.alignment;
                        int oldFontSize = GUI.skin.label.fontSize;
                        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                        GUI.skin.label.fontSize = 12;
                        Graphics.DrawTexture(labelBackground, blackTex);
                        float total = 0f;
                        for (int i = 0; i < timestamps.Count - 1; i++)
                            total += (timestamps[i + 1] - timestamps[i]);
                        float framesPerMinute = 60f / (total / (float)(timestamps.Count));
                        float framesLeft = (float)secondsToRecord * (float)effectiveFrameRateInt - frameCounter;
                        float minutesLeft = (framesLeft / framesPerMinute);
                        int hoursLeft = ((int)minutesLeft / 60);
                        int subHourMinutesleft = (int)minutesLeft - hoursLeft * 60;
                        int secondsLeft = (int)((minutesLeft - Mathf.Floor(minutesLeft)) * 60f);
                        if (hoursLeft > 0)
                            GUI.Label(textLabel, "" + frameCounter + "/" + secondsToRecord * effectiveFrameRateInt + " - " + framesPerMinute.ToString("0.00") + " FPM - " + hoursLeft.ToString() + "h" + subHourMinutesleft.ToString("D2") + "m" + secondsLeft.ToString("D2") + "s left", GUI.skin.label);
                        else
                            GUI.Label(textLabel, "" + frameCounter + "/" + secondsToRecord * effectiveFrameRateInt + " - " + framesPerMinute.ToString("0.00") + " FPM - " + subHourMinutesleft.ToString("D2") + "m" + secondsLeft.ToString("D2") + "s left", GUI.skin.label);
                        GUI.skin.label.alignment = oldAlignment;
                        GUI.skin.label.fontSize = oldFontSize;
                    }
                }

                GL.sRGBWrite = false;
            }
        }

        private void Update()
        {
            try
            {
                handledPre = false;
                handledPost = false;


                if (delayedInitCounter > 0)
                {
                    delayedInitCounter--;
                    if (delayedInitCounter == 0)
                        DelayedInit();
                }


                if (leaveEmptySphereVisible.val)
                {
                    if (sphereObject != null)
                    {
                        sphereObject.active = true;
                        sphereObject.GetComponent<MeshRenderer>().enabled = true;
                    }

                    if (targetSphereObject != null)
                    {
                        targetSphereObject.gameObject.active = true;
                        targetSphereObject.GetComponent<MeshRenderer>().enabled = true;
                    }
                }

                insideUpdate = true;
                if (videoPlayer != null && videoPlayer.isPlaying)
                    timeSlider.slider.value = (float)videoPlayer.time;// timeChooser.val = (float)videoPlayer.time;
                insideUpdate = false;
                if (videoPlayer != null && videoNotPausedYet && videoPlayer.isPlaying)
                {
                    if (videoPlayer.frame > 2)
                    {
                        videoPlayer.Pause();
                        videoNotPausedYet = false;
                    }
                }

                if (renderEndedLastFrame)
                {
                    renderEndedLastFrame = false;
                    if (!bFlatRender && previewChooser.val)
                    {
                        BeginVRPreview();
                        myNeedSetup = true;
                        oldOverlap = -1;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                    EndRender();

                if (Input.GetKeyDown(KeyCode.F9))
                    TakeSingleScreenshot();

                if (Input.GetKeyDown(KeyCode.F10))
                    StartPlaybackAndRecordVideo();

                if (Input.GetKeyDown(KeyCode.F11))
                    RecordVideo();

                if (Input.GetKeyDown(KeyCode.F12))
                    SeekToBeginning();

                if (myNeedSetup)
                    Setup();
            }
            catch (Exception e)
            {
                SuperController.LogError(e.ToString());
            }
        }

        private static float Lanczos(float a, float x)
        {
            if (-0.001f < x && x < 0.001f)
                return 1.0f;
            else if (x <= -a || x >= a)
                return 0.0f;

            float xPi = Mathf.PI * x;
            return a * Mathf.Sin(xPi) * Mathf.Sin(xPi / a) / (xPi * xPi);
        }

        private static float LanczosIntegral(float a, float x1, float x2)
        {
            const int integralSamples = 8;
            float sum = 0;
            for (int i = 0; i <= integralSamples; ++i)
            {
                float x = Mathf.Lerp(x1, x2, (float)i / integralSamples);
                sum += Lanczos(a, x);
            }
            return sum / integralSamples;
        }

        private void SetupDownsampleMaterial()
        {
            const int maxCoefficients = 24;
            float[] coefficients = new float[maxCoefficients];
            if (myKernelMode == KERNEL_LINEAR)
            {
                for (int i = 0; i < flatSupersampling; ++i)
                    coefficients[i] = 1.0f / flatSupersampling;
                for (int i = flatSupersampling; i < maxCoefficients; ++i)
                    coefficients[i] = 0.0f;
                myConvolutionMaterial.SetFloatArray("_Coefficients", coefficients);
                myConvolutionMaterial.SetInt("_CoefficientsCount", flatSupersampling);
            }
            else if (myKernelMode == KERNEL_LANCZOS3)
            {
                float a = 3;
                float kernelWidth = 4 * flatSupersampling;
                int kernelSamples = (int)Mathf.Ceil(2.5f * flatSupersampling);
                float kernelScale = a / (kernelWidth * 0.5f);

                float sum = 0;
                for (int i = 0; i < kernelSamples; ++i)
                {
                    float x = i - kernelSamples * 0.5f;
                    float x1 = kernelScale * x;
                    float x2 = kernelScale * (x + 1.0f);
                    float k = LanczosIntegral(a, x1, x2);
                    coefficients[i] = k;
                    sum += k;
                }
                for (int i = 0; i < kernelSamples; ++i)
                    coefficients[i] /= sum;
                for (int i = kernelSamples; i < maxCoefficients; ++i)
                    coefficients[i] = 0.0f;
                myConvolutionMaterial.SetFloatArray("_Coefficients", coefficients);
                myConvolutionMaterial.SetInt("_CoefficientsCount", kernelSamples);
            }
        }

        void FlatDownsample()
        {
            if (myConvolutionMaterial == null)
                return;

            Vector4 scaleWidth = new Vector4(1.0f / flatRenderTex.width, 0, 0, 0);
            Vector4 scaleHeight = new Vector4(0, 1.0f / flatRenderTex.height, 0, 0);
            bool horizontal = flatRenderTex.width >= flatRenderTex.height;

            myConvolutionMaterial.SetVector("_TexelSize", horizontal ? scaleWidth : scaleHeight);
            Graphics.Blit(flatRenderTex, passTexture, myConvolutionMaterial, 0);
            myConvolutionMaterial.SetVector("_TexelSize", horizontal ? scaleHeight : scaleWidth);
            Graphics.Blit(passTexture, outputRenderTexture, myConvolutionMaterial, 0);
        }

        void RenderTexToTex2D(RenderTexture renderTex, Texture2D tex2D)
        {
            RenderTexture previousRT = RenderTexture.active;
            Rect sourceRect = new Rect(0, 0, renderTex.width, renderTex.width);
            RenderTexture.active = renderTex;
            tex2D.ReadPixels(sourceRect, 0, 0, false);
            tex2D.Apply();
            RenderTexture.active = previousRT;
        }

        void SyncCameraTransform()
        {
            if (renderCamParentObj != null)
            {
                renderCamParentObj.transform.position = containingAtom.mainController.transform.position;
                if (cameraTarget != null)
                    renderCamParentObj.transform.LookAt(cameraTarget, containingAtom.mainController.transform.up);
                else
                    renderCamParentObj.transform.rotation = containingAtom.mainController.transform.rotation;
            }
            if (previewCamera != null)
            {
                previewCamera.transform.position = containingAtom.mainController.transform.position;
                if (cameraTarget != null)
                    previewCamera.transform.LookAt(cameraTarget, containingAtom.mainController.transform.up);
                else
                    previewCamera.transform.rotation = containingAtom.mainController.transform.rotation;
            }
        }

        void OnPostRenderCallback(Camera cam)
        {
            if (handledPost)
                return;

            handledPost = true;
        }


        void SyncCommandBuffers(Camera cam)
        {
            if (cam == null)
                return;
            if (!useCommandBuffers.val)
            {
                cam.depthTextureMode = DepthTextureMode.None;
                cam.RemoveAllCommandBuffers();
                return;
            }

            Camera sourceCam = Camera.main; // SuperController.singleton.hiResScreenshotCamera;
            CameraEvent[] camEvents = (CameraEvent[])Enum.GetValues(typeof(CameraEvent));
            cam.RemoveAllCommandBuffers();
            foreach (CameraEvent camEvent in camEvents)
            {
                CommandBuffer[] buffers = sourceCam.GetCommandBuffers(camEvent);
                foreach (CommandBuffer buff in buffers)
                {
                    if (buff.name.Contains("Subsurface"))
                        cam.depthTextureMode = DepthTextureMode.Depth;
                    cam.AddCommandBuffer(camEvent, buff);
                }
            }
        }

        bool usingPostProcessing;
        void SyncPostProcessing(Camera cam)
        {
            if (cam == null)
                return;
            PostProcessingBehaviour myBehaviour = cam.GetComponent<PostProcessingBehaviour>();
            if (!usePostProcessing.val)
            {
                usingPostProcessing = false;
                if (myBehaviour != null)
                    myBehaviour.enabled = false;
                return;
            }

            Camera sourceCam = Camera.main;
            PostProcessingBehaviour sourceBehaviour = sourceCam.GetComponent<PostProcessingBehaviour>();

            if (sourceBehaviour == null)
            {
                if (myBehaviour != null)
                    myBehaviour.enabled = false;

                usingPostProcessing = false;
            }
            else
            {
                if (myBehaviour == null)
                    myBehaviour = cam.gameObject.AddComponent<PostProcessingBehaviour>();

                myBehaviour.enabled = sourceBehaviour.enabled;
                myBehaviour.profile = sourceBehaviour.profile;

                usingPostProcessing = myBehaviour.enabled;
            }
        }

        NativeArray<float> audioBufferGeneric;
        NativeArray<float> audioBuffer64;
        NativeArray<float> audioBuffer128;
        NativeArray<float> audioBuffer256;
        NativeArray<float> audioBuffer512;
        NativeArray<float> audioBuffer1024;
        NativeArray<float> audioBuffer2048;
        NativeArray<float> audioBuffer4096;
        NativeArray<float> audioBuffer8192;
        NativeArray<float> audioBuffer16384;
        List<short> pcmAudioData, totalPcmAudioData;
        bool bActuallyRecordAudio;
        GameObject mainListener;
        int audioChannelCount = 2;
        int samplesPerFrame;
        int sampleRange;

        void StartAudioRecording()
        {
            if (AudioSettings.speakerMode != AudioSpeakerMode.Mono && AudioSettings.speakerMode != AudioSpeakerMode.Stereo)
            {
                SuperController.LogError("VRRenderer: Only mono and stereo sound recording supported");
                bActuallyRecordAudio = false;
                return;
            }

            AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;
            mainListener = GameObject.FindObjectOfType<AudioListener>()?.gameObject;
            if (mainListener != null)
            {
                velocityUpdateMode = mainListener.GetComponent<AudioListener>().velocityUpdateMode;
                mainListener.GetComponent<AudioListener>().enabled = false;
                Destroy(mainListener.GetComponent<AudioListener>());
            }

            renderCamParentObj.AddComponent<AudioListener>();
            renderCamParentObj.GetComponent<AudioListener>().velocityUpdateMode = velocityUpdateMode;

            sampleRange = (int)audioSampleRangeChooser.val;
            audioChannelCount = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2;
            if (totalPcmAudioData == null)
                totalPcmAudioData = new List<short>(AudioSettings.outputSampleRate * audioChannelCount * secondsToRecord);
            if (pcmAudioData == null)
                pcmAudioData = new List<short>(AudioSettings.outputSampleRate * audioChannelCount * secondsToRecord);
            samplesPerFrame = AudioSettings.outputSampleRate * audioChannelCount / frameRateInt;

            audioBuffer64 = new NativeArray<float>(64, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer128 = new NativeArray<float>(128, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer256 = new NativeArray<float>(256, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer512 = new NativeArray<float>(512, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer1024 = new NativeArray<float>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer2048 = new NativeArray<float>(2048, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer4096 = new NativeArray<float>(4096, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer8192 = new NativeArray<float>(8192, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBuffer16384 = new NativeArray<float>(16384, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            audioBufferGeneric = default(NativeArray<float>);

            bActuallyRecordAudio = true;
            AudioRenderer.Start();
        }

        void EndAudioRecording()
        {
            AudioRenderer.Stop();
            audioBuffer64.Dispose();
            audioBuffer128.Dispose();
            audioBuffer256.Dispose();
            audioBuffer512.Dispose();
            audioBuffer1024.Dispose();
            audioBuffer2048.Dispose();
            audioBuffer4096.Dispose();
            audioBuffer8192.Dispose();
            audioBuffer16384.Dispose();

            AudioVelocityUpdateMode velocityUpdateMode = renderCamParentObj.GetComponent<AudioListener>().velocityUpdateMode;
            renderCamParentObj.GetComponent<AudioListener>().enabled = false;
            Destroy(renderCamParentObj.GetComponent<AudioListener>());
            if (mainListener != null)
            {
                mainListener.AddComponent<AudioListener>();
                mainListener.GetComponent<AudioListener>().velocityUpdateMode = velocityUpdateMode;
            }
            totalPcmAudioData.AddRange(pcmAudioData);
            pcmAudioData = null;
            WriteWavFile(baseFilename, totalPcmAudioData.ToArray());
        }

        void RecordFrameAudio()
        {
            int currentSamples = AudioRenderer.GetSampleCountForCaptureFrame() * audioChannelCount;
            NativeArray<float> currentBuffer;
            if (currentSamples == 64) currentBuffer = audioBuffer64;
            else if (currentSamples == 128) currentBuffer = audioBuffer128;
            else if (currentSamples == 256) currentBuffer = audioBuffer256;
            else if (currentSamples == 512) currentBuffer = audioBuffer512;
            else if (currentSamples == 1024) currentBuffer = audioBuffer1024;
            else if (currentSamples == 2048) currentBuffer = audioBuffer2048;
            else if (currentSamples == 4096) currentBuffer = audioBuffer4096;
            else if (currentSamples == 8192) currentBuffer = audioBuffer8192;
            else if (currentSamples == 16384) currentBuffer = audioBuffer16384;
            else
            {
                audioBufferGeneric = new NativeArray<float>(currentSamples, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                currentBuffer = audioBufferGeneric;
            }

            AudioRenderer.Render(currentBuffer);
            for (int i = 0; i < currentSamples; i++)
            {
                pcmAudioData.Add((short)(Mathf.Clamp(Mathf.Round((float)currentBuffer[i] * maxShort), -sampleRange, sampleRange - 1)));
            }
            if (audioBufferGeneric.IsCreated)
                audioBufferGeneric.Dispose();
        }

        void WriteWavFile(string filename, short[] pcmSamples)
        {
            if (pcmSamples.Length == 0)
            {
                SuperController.LogError("VRRenderer: No audio data recorded");
                return;
            }

            int sampleCount = pcmSamples.Length / audioChannelCount;
            int channelCount = audioChannelCount;
            int sampleRate = AudioSettings.outputSampleRate;

            int bytesPerSample = sizeof(short);
            int headerLength = 44;
            int audioDataLength = sampleCount * channelCount * bytesPerSample;
            int fileLength = headerLength + audioDataLength;
            byte[] bytes = new byte[fileLength];
            List<byte> header = new List<byte>(headerLength);
            header.AddRange(Encoding.ASCII.GetBytes("RIFF"));
            header.AddRange(BitConverter.GetBytes((uint)(fileLength - 8)));
            header.AddRange(Encoding.ASCII.GetBytes("WAVEfmt "));
            header.AddRange(BitConverter.GetBytes((uint)16));
            header.AddRange(BitConverter.GetBytes((ushort)1));
            header.AddRange(BitConverter.GetBytes((ushort)channelCount));
            header.AddRange(BitConverter.GetBytes((uint)sampleRate));
            header.AddRange(BitConverter.GetBytes((uint)(sampleRate * bytesPerSample * channelCount)));
            header.AddRange(BitConverter.GetBytes((ushort)(channelCount * bytesPerSample)));
            header.AddRange(BitConverter.GetBytes((ushort)(bytesPerSample * 8)));
            header.AddRange(Encoding.ASCII.GetBytes("data"));
            header.AddRange(BitConverter.GetBytes((uint)audioDataLength));
            Array.Copy(header.ToArray(), bytes, header.Count);
            Buffer.BlockCopy(pcmSamples, 0, bytes, header.Count, bytesPerSample * pcmSamples.Length);
            FileManagerSecure.WriteAllBytes(myDirectory + filename + "_" + (frameCounter - 1).ToString("D6") + ".wav", bytes);
        }

        void OnPreRenderCallback(Camera cam)
        {
            if (handledPre)
                return;

            handledPre = true;

            if (bRendering && bBvhRender)
            {
                UpdateBVHRender();
                return;
            }

            if (!bRendering)
            {
                SyncPostProcessing(previewCamera);
                SyncCommandBuffers(previewCamera);
            }

            SyncPostProcessing(renderCam);
            SyncCommandBuffers(renderCam);

            SyncCameraTransform();

            if (bRendering)
            {
                UpdateRenderCamera();
                if (recordAudioChooser.val && bActuallyRecordAudio)
                    RecordFrameAudio();
            }
            else if (renderCam != null && !bFlatRender && previewChooser.val && renderCamParentObj != null)
            {
                UpdateVRPreview();
            }
            else if (previewChooser.val && previewCamera != null && bFlatRender)
            {
                UpdateFlatPreview();
            }
        }

        void UpdateVRPreview()
        {
            float passedTime = Time.realtimeSinceStartup - lastVRPreviewUpdate;
            if (passedTime > (1f / vrPreviewFPS))
            {
                if (lastVRPreviewUpdate + 1f / vrPreviewFPS > Time.realtimeSinceStartup)
                    lastVRPreviewUpdate += 1f / vrPreviewFPS;
                else
                    lastVRPreviewUpdate = Time.realtimeSinceStartup;
            }
            else
            {
                //  renderCamParentObj.SetActive(false);
                return;
            }

            renderCam.clearFlags = CameraClearFlags.SolidColor;
            Color color = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);
            renderCam.backgroundColor = new Color(color.r, color.g, color.b, 1f);

            if (bStereoRender)
            {
                if (stereoMode == STEREO_STATIC || stereoMode == STEREO_PANORAMIC && renderFaces.Length == 6)
                {
                    Vector3 eyePosition = Vector3.zero;
                    if (GetCurrentBackground() != null && backgroundStereoLayout != BG_MONO)
                        eyePosition = Vector3.left * ipd / 2000f;
                    RenderCubemap(renderCam, renderFaces, eyePosition);
                    CubemapToEquirectShader(renderFaces, equirectL);
                }
                else if (stereoMode == STEREO_SQUARE && renderFaces.Length == 12)
                {
                    RenderSquareMap(renderCam, renderFaces, Vector3.left * ipd / 2000f);
                    SquareMapToEquirectShader(renderFaces, equirectL);
                }
                else if (stereoMode == STEREO_TRIANGLE && renderFaces.Length == 9)
                {
                    RenderTriangleMap(renderCam, renderFaces, Vector3.left * ipd / 2000f);
                    TriangleMapToEquirectShader(renderFaces, equirectL);
                }
            }
            else if (renderFaces.Length == 6)
            {
                RenderCubemap(renderCam, renderFaces, Vector3.zero);
                CubemapToEquirectShader(renderFaces, equirectL);
            }
        }

        void UpdateFlatPreview()
        {
            bool bPreserveAlphaInRender = preserveTransparencyChooser.val || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null));
            bool bPreserveAlphaNow = bPreserveAlphaInRender || (hideBackgroundColorOnPreviewOnly.val);

            previewCamera.fieldOfView = flatVerticalFov;

            if (bPreserveAlphaNow)
            {
                previewCamera.targetTexture = previewTexBlack;
                previewCamera.backgroundColor = Color.black;
                previewCamera.Render();
                previewCamera.targetTexture = previewTexWhite;
                previewCamera.backgroundColor = Color.white;
                previewCamera.Render();
                AlphaFromDifferenceShader(previewTexBlack, previewTexWhite, previewTex);
            }
            else
            {
                Color color = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);
                previewCamera.backgroundColor = new Color(color.r, color.g, color.b, 1f);
                previewCamera.Render();
            }
        }

        void UpdateRenderCamera()
        {
            if (!PrepareFrame())
                return;

            RenderFrame();

            ProcessFrame();
        }

        void ProcessFrame()
        {
            if (bFlatRender)
            {
                if (flatSupersampling > 1)
                {
                    FlatDownsample();
                    RenderTexToTex2D(outputRenderTexture, finalOutputTexture);
                }
                else
                {
                    RenderTexToTex2D(flatRenderTex, finalOutputTexture);
                }
            }
            if (bRecordVideo)
            {
                SaveRenderAsFile(myFileFormat, baseFilename + "_" + frameCounter.ToString("D6"), finalOutputTexture);

                timestamps.Add(Time.realtimeSinceStartup);

                frameCounter++;

                if (frameCounter == effectiveFrameRateInt * secondsToRecord)
                {
                    EndRender();
                    return;
                }
            }
            else
            {
                SaveRenderAsFile(myFileFormat, baseFilename, finalOutputTexture);
                EndRender();
                return;
            }
        }

        long lastFrameReceived;
        void VideoFrameReady(VideoPlayer source, long frameIndex)
        {
            //   SuperController.LogMessage("received frame " + frameIndex + " waiting for " + frameWaitedFor + " while " + (bRendering ? "" : "not ") + "rendering");
            //   if (frameIndex != frameWaitedFor)
            //       return;
            if (bRendering) // lastFrameReceived != frameIndex
            {
                videoFrameReady = true;
                lastFrameReceived = frameIndex;
            }
        }

        void StepVideo()
        {
            frameWaitStartTime = Time.realtimeSinceStartup;
            videoFrameReady = false;
            Time.timeScale = 0f;
            float timePassed = (float)frameCounter / (float)effectiveFrameRateInt;
            float videoLength = (float)videoPlayer.frameCount / (float)videoPlayer.frameRate;
            if (loopChooser.val)
            {
                videoPlayer.time = (videoTimeOnRenderBegin + timePassed) % videoLength;
                frameWaitedFor = (int)(((videoTimeOnRenderBegin + timePassed) % videoLength) * videoPlayer.frameRate);
            }
            else
            {
                if (videoTimeOnRenderBegin + timePassed > videoLength - 0.1f)
                    shouldAdvanceVideo = false;
                else
                {
                    videoPlayer.time = videoTimeOnRenderBegin + timePassed;
                    frameWaitedFor = (int)(((videoTimeOnRenderBegin + timePassed)) * videoPlayer.frameRate);
                }
            }
        }

        bool PrepareFrame()
        {
            if (framesToSkip > 0)
            {
                frameSkipCounter++;
                if (frameSkipCounter == framesToSkip)
                {
                    frameSkipCounter = 0;
                    if (renderCamParentObj != null)
                    {
                        //    renderCamParentObj.SetActive(true);
                    }
                }
                else
                {
                    if (renderCamParentObj != null)
                    {
                        //    renderCamParentObj.SetActive(false);
                    }
                    return false;
                }
            }

            if (shouldAdvanceVideo && renderBackgroundChooser.val && videoPlayer != null)
            {
                if (Time.timeScale == 0f && !videoFrameReady)
                {
                    if (Time.realtimeSinceStartup - frameWaitStartTime > 5f)
                    {
                        SuperController.LogMessage("frame " + Time.frameCount + "VideoPlayer cannot seek to frame " + frameWaitedFor + " - skipping.");
                        videoFrameReady = true; // proceed to render with old frame
                    }
                    else
                        return false;
                }
                if (Time.timeScale == 0f && videoFrameReady)
                {
                    // step animation 1 frame
                    Time.timeScale = oldTimeScale;
                    return false;
                }
                else if (Time.timeScale == oldTimeScale && videoFrameReady)
                {
                    // proceed to render
                }
                else if (Time.timeScale == oldTimeScale && !videoFrameReady)
                {
                    // should never happen
                }

                StepVideo();
            }

            return true;
        }

        Texture GetCurrentBackground()
        {
            if (renderBackgroundChooser.val && (previewBackground != null || (videoPlayer != null)))
            {
                if (previewBackground != null)
                    return previewBackground;
                else if (videoPlayer != null)
                    return videoPlayer.texture;
                else
                    return null;
            }
            else
                return null;
        }

        void RenderFrame()
        {
            renderCam.clearFlags = CameraClearFlags.SolidColor;
            Color color = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);
            renderCam.backgroundColor = new Color(color.r, color.g, color.b, 1f);

            Texture background = GetCurrentBackground();

            if (bStereoRender)
            {
                if (stereoMode == STEREO_STATIC)
                {
                    RenderCubemap(renderCam, renderFaces, Vector3.left * ipd / 2000f);
                    CubemapToEquirectShader(renderFaces, equirectL, background);
                    RenderCubemap(renderCam, renderFaces, Vector3.right * ipd / 2000f);
                    CubemapToEquirectShader(renderFaces, equirectR, background);
                }
                else if (stereoMode == STEREO_SQUARE)
                {
                    RenderSquareMap(renderCam, renderFaces, Vector3.left * ipd / 2000f);
                    SquareMapToEquirectShader(renderFaces, equirectL);
                    RenderSquareMap(renderCam, renderFaces, Vector3.right * ipd / 2000f);
                    SquareMapToEquirectShader(renderFaces, equirectR);
                }
                else if (stereoMode == STEREO_TRIANGLE)
                {
                    RenderTriangleMap(renderCam, renderFaces, Vector3.left * ipd / 2000f);
                    TriangleMapToEquirectShader(renderFaces, equirectL);
                    RenderTriangleMap(renderCam, renderFaces, Vector3.right * ipd / 2000f);
                    TriangleMapToEquirectShader(renderFaces, equirectR);
                }
                else if (stereoMode == STEREO_PANORAMIC)
                {
                    RenderPanoramicStereoMap(renderCam, finalOutputTexture, Vector3.left * ipd / 2000f, b180Degrees ? 180f : 360f);
                }

                if (stereoMode != STEREO_PANORAMIC)
                    MergeSidesToTexture(equirectL, equirectR, finalOutputTexture, b180Degrees);
            }
            else
            {
                if (bFlatRender)
                {
                    if ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || background != null)
                    {
                        renderCam.backgroundColor = Color.black;
                        RenderFlatCamera(renderCam, flatRenderTexBlack, flatVerticalFov);
                        renderCam.backgroundColor = Color.white;
                        RenderFlatCamera(renderCam, flatRenderTexWhite, flatVerticalFov);
                        AlphaFromDifferenceShader(flatRenderTexBlack, flatRenderTexWhite, flatRenderTex, background, true);
                    }
                    else
                    {
                        RenderFlatCamera(renderCam, flatRenderTex, flatVerticalFov);
                    }
                }
                else
                {
                    RenderCubemap(renderCam, renderFaces, Vector3.zero);
                    CubemapToEquirectShader(renderFaces, equirectL, background);
                    RenderTexToTex2D(equirectL, finalOutputTexture);
                }
            }
        }


        void RenderFlatCamera(Camera cam, RenderTexture texture, float verticalFov)
        {
            cam.targetTexture = texture;
            cam.fieldOfView = verticalFov;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
            cam.Render();
        }

        void MergeSidesToTexture(RenderTexture leftTex, RenderTexture rightTex, Texture2D outputTex, bool layout180Degrees)
        {
            RenderTexture previousRT = RenderTexture.active;
            Rect sourceRect = new Rect(0, 0, leftTex.width, leftTex.height);
            if (layout180Degrees)
            {
                RenderTexture.active = leftTex;
                outputTex.ReadPixels(sourceRect, 0, 0, false);
                RenderTexture.active = rightTex;
                outputTex.ReadPixels(sourceRect, outputTex.width / 2, 0, false);
            }
            else
            {
                RenderTexture.active = rightTex;
                outputTex.ReadPixels(sourceRect, 0, 0, false);
                RenderTexture.active = leftTex;
                outputTex.ReadPixels(sourceRect, 0, outputTex.height / 2, false);
            }
            outputTex.Apply();
            RenderTexture.active = previousRT;
        }

        private void TakeSingleScreenshot()
        {
            if (bBvhRender)
            {
                SuperController.LogMessage("VRRenderer: Cannot take screenshot in BVH mode.");
                return;
            }
            if (!CanTakeScreenshot())
                return;

            bRecordVideo = false;
            BeginRender();
        }

        private void RecordVideo()
        {
            if (!CanTakeScreenshot())
                return;

            if (!pauseVideoChooser.val)
                frameCounter = 0;

            if (frameCounter == 0)
            {
                pcmAudioData = null;
                totalPcmAudioData = null;
                lastFilename = "";
            }

            bRecordVideo = true;
            BeginRender();
        }

        private void StartPlaybackAndRecordVideo()
        {
            if (!CanTakeScreenshot())
                return;

            SuperController.singleton.motionAnimationMaster.StartPlayback();
            unfrozeOnStart = false;
            if (unfreezeOnPlaybackChooser.val)
            {
                if (SuperController.singleton.freezeAnimation)
                {
                    SuperController.singleton.SetFreezeAnimation(false);
                    unfrozeOnStart = true;
                }
                else
                    unfrozeOnStart = false;
            }
            RecordVideo();
        }

        private void SeekToBeginning()
        {
            SuperController.singleton.motionAnimationMaster.SeekToBeginning();
        }


        private bool CanTakeScreenshot()
        {
            if (bRendering)
            {
                SuperController.LogMessage("VRRenderer: Already recording, end render manually first (Escape key).");
                return false;
            }

            if (pauseVideoChooser.val && frameCounter >= secondsToRecord * effectiveFrameRateInt)
            {
                SuperController.LogMessage("VRRenderer: Reached end of recording time span, increase seconds to record or start new recording.");
                return false;
            };

            int[] checkSizes = new int[]
            {
                finalResolution.x,
                finalResolution.y,
                finalResolution.x * flatSupersampling,
                finalResolution.y * flatSupersampling,
                cubemapSideLength
            };

            foreach (var size in checkSizes)
            {
                if (size > SystemInfo.maxTextureSize)
                {
                    SuperController.LogMessage("VRRenderer: Selected resolution or supersampling exceeds capabilities of your GPU.");
                    return false;
                }
            }

            if (myMemoryEstimate * 1024f > SystemInfo.graphicsMemorySize - 3072)
            {
                SuperController.LogMessage("VRRenderer: Selected settings would consume more VRAM than your GPU likely has available.");
                if (!VRAMWarningChooser.val)
                    return false;
            }

            if (!enabled || !gameObject.activeInHierarchy)
            {
                SuperController.LogMessage("VRRenderer can only take screenshots when both the plugin and the atom are enabled.");
                return false;
            }

            return true;
        }

        RenderTexture GetRenderTexture(int width, int height, int msaa, bool depth)
        {
            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(width, height, renderTextureFormat, depth ? 24 : 0);
            descriptor.msaaSamples = msaa;
            descriptor.autoGenerateMips = false;
            descriptor.useMipMap = false;
            descriptor.sRGB = true;
            RenderTexture texture = new RenderTexture(descriptor);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            return texture;
        }

        public string GetFilename()
        {
            return DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + "-"
                + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2");
        }

        public void SaveRenderAsFile(int fileFormat, string filename, Texture2D tex)
        {
            filename += (fileFormat == FORMAT_JPG) ? ".jpg" : ".png";
            byte[] bytes = (fileFormat == FORMAT_JPG) ? tex.EncodeToJPG(jpegQuality) : tex.EncodeToPNG();
            FileManagerSecure.WriteAllBytes(myDirectory + filename, bytes);
        }

        void BeginVRPreview()
        {
            bRendering = false;

            int width = previewTex.width;
            int height = previewTex.height;
            int previewCubemapSideLength = (int)(width * ((float)cubemapSideLength / (float)finalResolution.x));

            if (previewCubemapSideLength < selectedCubemapSideLength)
                previewCubemapSideLength = (int)Mathf.Min(width * 1.5f, selectedCubemapSideLength); // don't preview cubemap resolution higher than preview width, but preview selected resolution if it exceeds resolution calculated by ratio


            if (renderCamParentObj == null)
                renderCamParentObj = new GameObject();
            if (renderCam == null)
                renderCam = CreateFlatCamera(renderCamParentObj.transform);

            SyncPostProcessing(renderCam);
            SyncCommandBuffers(renderCam);

            SyncCameraTransform();

            if (_equirectMat == null || _equirectMatAlpha == null)
            {
                SuperController.LogError("LilyRender shader not loaded.");
                return;
            }

            equirectL = GetRenderTexture(width, height, 1, false);

            int sideCount = 6;
            if (bStereoRender)
            {
                if (stereoMode == STEREO_STATIC || stereoMode == STEREO_PANORAMIC)
                    sideCount = 6;
                else if (stereoMode == STEREO_SQUARE)
                    sideCount = 12;
                else if (stereoMode == STEREO_TRIANGLE)
                    sideCount = 9;
            }

            // always transparency support in preview
            blackBgRenderTex = GetRenderTexture(previewCubemapSideLength, previewCubemapSideLength, myMsaaLevel, true);
            whiteBgRenderTex = GetRenderTexture(previewCubemapSideLength, previewCubemapSideLength, myMsaaLevel, true);

            renderFaces = new RenderTexture[sideCount];

            for (int i = 0; i < sideCount; i++)
            {
                if (bStereoRender && stereoMode == STEREO_SQUARE && i % 3 != 1) // bottom or top texture quarter res
                    renderFaces[i] = GetRenderTexture(previewCubemapSideLength / 2, previewCubemapSideLength / 2, myMsaaLevel, true);
                else
                    renderFaces[i] = GetRenderTexture(previewCubemapSideLength, previewCubemapSideLength, myMsaaLevel, true);
            }
        }

        void EndVRPreview()
        {
            renderCamParentObj.SetActive(false);

            Destroy(renderCamParentObj);
            renderCamParentObj = null;

            Destroy(renderCam);
            renderCam = null;

            foreach (var rt in renderFaces)
                Destroy(rt);
            Destroy(equirectL);

            Destroy(blackBgRenderTex);
            blackBgRenderTex = null;
            Destroy(whiteBgRenderTex);
            whiteBgRenderTex = null;
        }

        string Vector3String(Vector3 v, bool bRotation, Quaternion2Angles.RotationOrder rotationOrder = Quaternion2Angles.RotationOrder.ZXY)
        {
            string f = "F9";

            if (bRotation)
            {
                switch (rotationOrder)
                {
                    case Quaternion2Angles.RotationOrder.XYZ:
                        return v.x.ToString(f) + " " + v.y.ToString(f) + " " + v.z.ToString(f);
                        break;
                    case Quaternion2Angles.RotationOrder.XZY:
                        return v.x.ToString(f) + " " + v.z.ToString(f) + " " + v.y.ToString(f);
                        break;
                    case Quaternion2Angles.RotationOrder.YXZ:
                        return v.y.ToString(f) + " " + v.x.ToString(f) + " " + v.z.ToString(f);
                        break;
                    case Quaternion2Angles.RotationOrder.YZX:
                        return v.y.ToString(f) + " " + v.z.ToString(f) + " " + v.x.ToString(f);
                        break;
                    case Quaternion2Angles.RotationOrder.ZXY:
                        return v.z.ToString(f) + " " + v.x.ToString(f) + " " + v.y.ToString(f);
                        break;
                    case Quaternion2Angles.RotationOrder.ZYX:
                        return v.z.ToString(f) + " " + v.y.ToString(f) + " " + v.x.ToString(f);
                        break;
                    default:
                        return "nothing";
                }
            }
            else
            {
                v *= dazBvhScaleChooser.val ? 100f : 1f;
                // unity is left-handed, bvh/daz/blender are right-handed
                v = new Vector3(-v.x, v.y, v.z);
                return v.x.ToString(f) + " " + v.y.ToString(f) + " " + v.z.ToString(f);
            }
        }

        class BvhPersonData
        {
            public DAZBones root = null;
            public DAZBone hipBone = null;
            public StringBuilder bvhOutput = new StringBuilder("HIERARCHY\n", (int)((11000 + VRRenderer.secondsToRecord * VRRenderer.frameRateInt * 2650) * 1.1f));
            public int bvhOutputFrameOffset = 0;
            public int frames = 0;
            public List<DAZBone> orderedBones = new List<DAZBone>();
            public Vector3 hipStartingPosition;
        }

        Dictionary<DAZBones, BvhPersonData> bvhData = new Dictionary<DAZBones, BvhPersonData>();

        string[] gen2BoneNames = new string[]
        {
            "hip", "pelvis", "abdomen", "chest", "Pectoral","neck", "head", "Jaw", "Eye", "tongue",
            "Collar", "Shldr", "ForeArm", "Hand", "Carpal", "Index", "Ring", "Mid", "Pinky", "Thumb",
            "Thigh", "Shin", "Foot", "Toe"
        };

        int treeDepth;
        Dictionary<DAZBone, List<DAZBone>> dazChildBones = new Dictionary<DAZBone, List<DAZBone>>();

        void EnumerateChildren(DAZBone bone)
        {
            DAZBone parent = bone.transform.parent.GetComponent<DAZBone>();

            if (!dazChildBones.ContainsKey(bone))
                dazChildBones[bone] = new List<DAZBone>();

            if (parent != null)
            {
                if (!dazChildBones.ContainsKey(parent))
                    dazChildBones[parent] = new List<DAZBone>();
                dazChildBones[parent].Add(bone);
            }

            foreach (Transform child in bone.transform)
            {
                bool skip = true;
                foreach (string s in gen2BoneNames)
                {
                    if (child.name.Contains(s))
                        skip = false;
                }
                if (skip)
                    continue;
                if (child.GetComponent<DAZBone>() != null)
                {
                    EnumerateChildren(child.GetComponent<DAZBone>());
                }
            }
        }

        void TraverseBone(DAZBone bone, BvhPersonData data)
        {
            string tabDeclare = "";

            for (int i = 0; i < treeDepth; i++)
                tabDeclare += "\t";

            string tabDefine = "\t" + tabDeclare;

            data.orderedBones.Add(bone);

            if (bone.name == "hip")
                data.bvhOutput.Append(tabDeclare + "ROOT hip\n{\n");
            else
                data.bvhOutput.Append(tabDeclare + "JOINT " + bone.name + "\n" + tabDeclare + "{\n");

            DAZBone parent = bone.transform.parent.GetComponent<DAZBone>();

            Vector3 targetPosition = bone.startingLocalPosition;

            if (parent != null && !bvhUseUnorientedSkeleton.val)
                targetPosition = bone.morphedWorldPosition - parent.morphedWorldPosition; // these are t-pose positions, not current positions

            data.bvhOutput.Append(tabDefine + "OFFSET " + Vector3String(targetPosition, false) + "\n");

            if (bone.name == "hip")
            {
                data.hipBone = bone;
                data.hipStartingPosition = bone.transform.localPosition;
            }

            string rotationOrderString = "";

            switch ((bone.rotationOrder))
            {
                case Quaternion2Angles.RotationOrder.XYZ: rotationOrderString = "Xrotation Yrotation Zrotation"; break;
                case Quaternion2Angles.RotationOrder.XZY: rotationOrderString = "Xrotation Zrotation Yrotation"; break;
                case Quaternion2Angles.RotationOrder.YXZ: rotationOrderString = "Yrotation Xrotation Zrotation"; break;
                case Quaternion2Angles.RotationOrder.YZX: rotationOrderString = "Yrotation Zrotation Xrotation"; break;
                case Quaternion2Angles.RotationOrder.ZXY: rotationOrderString = "Zrotation Xrotation Yrotation"; break;
                case Quaternion2Angles.RotationOrder.ZYX: rotationOrderString = "Zrotation Yrotation Xrotation"; break;
            }

            if (bone.name == "hip")
                data.bvhOutput.Append(tabDefine + "CHANNELS 6 Xposition Yposition Zposition " + rotationOrderString + "\n");
            else
                data.bvhOutput.Append(tabDefine + "CHANNELS 3 " + rotationOrderString + "\n");

            if ((!bone.name.Contains("Toe") && bone.name.Contains("3")) || bone.name.Contains("SmallToe") || bone.name.Contains("BigToe") || bone.name.Contains("Pectoral") || bone.name.Contains("Eye") || bone.name.Contains("upperJaw") || bone.name.Contains("tongueTip"))
            {
                if (endSites.ContainsKey(bone))
                {
                    data.bvhOutput.Append(tabDefine + "End Site\n" + tabDefine + "{\n");
                    data.bvhOutput.Append(tabDefine + "\t" + "OFFSET " + Vector3String(endSites[bone].position - adjustedRestSkeleton[bone].transform.position, false) + "\n");
                    data.bvhOutput.Append(tabDefine + "}\n");
                }
            }

            foreach (DAZBone child in dazChildBones[bone])
            {
                treeDepth++;
                TraverseBone(child, data);
            }

            data.bvhOutput.Append(tabDeclare + "}\n");
            treeDepth--;
        }

        Dictionary<DAZBone, Transform> endSites = new Dictionary<DAZBone, Transform>();

        void AddEndSites(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict)
        {
            if ((!bone.name.Contains("Toe") && bone.name.Contains("3")) || bone.name.Contains("SmallToe") || bone.name.Contains("BigToe") || bone.name.Contains("Pectoral"))
            {
                foreach (Transform child in bone.transform)
                {
                    CapsuleCollider collider = child.gameObject.GetComponent<CapsuleCollider>();
                    if (collider != null)
                    {
                        GameObject endSiteObject = new GameObject();
                        endSiteObject.transform.parent = copyDict[bone].transform;
                        endSites[bone] = endSiteObject.transform;

                        int directionAxis = collider.direction;
                        Vector3 colliderDirection;
                        if (directionAxis == 0) // x
                            colliderDirection = Vector3.right;
                        else if (directionAxis == 1) // y
                            colliderDirection = Vector3.up;
                        else // z
                            colliderDirection = Vector3.forward;

                        Vector3 localPos = collider.transform.localPosition;
                        if (bone.name.StartsWith("l"))
                        {
                            endSiteObject.transform.localPosition = new Vector3(-localPos.x, localPos.y, localPos.z) + collider.center + colliderDirection * collider.height / 2f;
                            endSiteObject.transform.localPosition = new Vector3(-endSiteObject.transform.localPosition.x, endSiteObject.transform.localPosition.y, endSiteObject.transform.localPosition.z);
                        }
                        else
                            endSiteObject.transform.localPosition = localPos + collider.center + colliderDirection * collider.height / 2f;

                        break;
                    }
                }
            }
            else if (bone.name.Contains("tongueTip"))
            {
                GameObject endSiteObject = new GameObject();
                endSiteObject.transform.parent = copyDict[bone].transform;
                endSites[bone] = endSiteObject.transform;
                endSiteObject.transform.position = copyDict[bone].transform.position + (copyDict[bone].transform.position - copyDict[bone].transform.parent.position);
            }
            else if (bone.name.Contains("Eye"))
            {
                GameObject endSiteObject = new GameObject();
                endSiteObject.transform.parent = copyDict[bone].transform;
                endSites[bone] = endSiteObject.transform;
                endSiteObject.transform.localPosition = Vector3.forward * 0.02f;
            }
            else if (bone.name.Contains("upperJaw"))
            {
                GameObject endSiteObject = new GameObject();
                endSiteObject.transform.parent = copyDict[bone].transform;
                endSites[bone] = endSiteObject.transform;
                endSiteObject.transform.localPosition = Vector3.forward * 0.04f;
            }
            foreach (DAZBone child in dazChildBones[bone])
                AddEndSites(child, copyDict);
        }

        void SetupBVH()
        {
            try
            {
                var bonesArray = GameObject.FindObjectsOfType<DAZBones>();
                bvhData.Clear();

                foreach (var bones in bonesArray)
                {
                    if (bones.name == "Genesis2Female" || bones.name == "Genesis2Male")
                    {
                        bvhData[bones] = new BvhPersonData();
                        bvhData[bones].root = bones;

                        treeDepth = 0;

                        foreach (Transform child in bones.transform)
                        {
                            if (child.GetComponent<DAZBone>() != null && child.name == "hip")
                            {
                                EnumerateChildren(child.GetComponent<DAZBone>());

                                MakeCopies(child.GetComponent<DAZBone>(), adjustedRestSkeleton);
                                SetCopiesToRestPose(child.GetComponent<DAZBone>(), adjustedRestSkeleton);
                                AddEndSites(child.GetComponent<DAZBone>(), adjustedRestSkeleton);

                                TraverseBone(child.GetComponent<DAZBone>(), bvhData[bones]);

                                StoreWorldPositions(child.GetComponent<DAZBone>(), adjustedRestSkeleton, posDict);
                                ZeroRotationsKeepPose(child.GetComponent<DAZBone>(), adjustedRestSkeleton, posDict);
                                AddRotationRefs(child.GetComponent<DAZBone>(), adjustedRestSkeleton);
                            }
                        }

                        bvhData[bones].bvhOutput.Append("MOTION\n");
                        bvhData[bones].bvhOutput.Append("Frames: ");

                        bvhData[bones].bvhOutputFrameOffset = bvhData[bones].bvhOutput.Length;
						string reciprocal = (1f / effectiveFrameRateInt).ToString("F8");
                        bvhData[bones].bvhOutput.Append("\nFrame Time: " + reciprocal.Substring(0, reciprocal.Length - 1) + "\n"); // skip last rounded digit to ensure the reciprocal's reciprocal is above the original int
                    }
                }
            }
            catch (Exception e)
            {
                SuperController.LogError(e.ToString());
            }
        }

        void WriteBVHs()
        {
            try
            {
                foreach (var pair in bvhData)
                {
                    var root = pair.Key;
                    var data = pair.Value;
                    data.bvhOutput.Insert(data.bvhOutputFrameOffset, data.frames);
                    data.bvhOutput.Length -= 1; // truncate last newline
                    FileManagerSecure.WriteAllText(myDirectory + baseFilename + "_" + root.containingAtom.name + ".bvh", data.bvhOutput.ToString());
                }
            }
            catch (Exception e)
            {
                SuperController.LogError(e.ToString());
            }
        }

        void UpdateBVHRender()
        {
            AddBVHFrame();

            if (recordAudioChooser.val && bActuallyRecordAudio)
                RecordFrameAudio();

            timestamps.Add(Time.realtimeSinceStartup);

            frameCounter++;

            if (frameCounter == frameRateInt * secondsToRecord)
            {
                EndRender();
                return;
            }
        }

        Dictionary<DAZBone, GameObject> adjustedRestSkeleton = new Dictionary<DAZBone, GameObject>();
        Dictionary<DAZBone, Transform> rotationRefs = new Dictionary<DAZBone, Transform>();
        Dictionary<DAZBone, Vector3> posDict = new Dictionary<DAZBone, Vector3>();

        void MakeCopies(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict)
        {
            if (!copyDict.ContainsKey(bone))
                copyDict[bone] = new GameObject();

            if (bone.transform.parent.GetComponent<DAZBone>() != null)
                copyDict[bone].transform.parent = copyDict[bone.transform.parent.GetComponent<DAZBone>()].transform;

            copyDict[bone].transform.position = bone.transform.position;
            copyDict[bone].transform.rotation = bone.transform.rotation;

            foreach (DAZBone child in dazChildBones[bone])
                MakeCopies(child, copyDict);
        }

        void StoreWorldPositions(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict, Dictionary<DAZBone, Vector3> positionDict)
        {
            positionDict[bone] = copyDict[bone].transform.position;

            foreach (DAZBone child in dazChildBones[bone])
                StoreWorldPositions(child, copyDict, positionDict);
        }

        void ZeroRotationsKeepPose(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict, Dictionary<DAZBone, Vector3> positionDict)
        {
            copyDict[bone].transform.rotation = Quaternion.identity;
            copyDict[bone].transform.position = positionDict[bone];

            foreach (DAZBone child in dazChildBones[bone])
                ZeroRotationsKeepPose(child, copyDict, positionDict);
        }

        void AddRotationRefs(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict)
        {
            rotationRefs[bone] = new GameObject().transform;
            rotationRefs[bone].parent = copyDict[bone].transform;
            Quaternion qf;
            if (bone.useUnityEulerOrientation)
                qf = Quaternion.Euler(bone.morphedWorldOrientation);
            else
                qf = Quaternion2Angles.EulerToQuaternion(bone.morphedWorldOrientation, Quaternion2Angles.RotationOrder.ZYX);

            rotationRefs[bone].rotation = qf;

            foreach (DAZBone child in dazChildBones[bone])
                AddRotationRefs(child, copyDict);
        }

        void SetCopiesToRestPose(DAZBone bone, Dictionary<DAZBone, GameObject> copyDict)
        {
            Quaternion qf;
            if (bone.useUnityEulerOrientation)
                qf = Quaternion.Euler(bone.morphedWorldOrientation);
            else
                qf = Quaternion2Angles.EulerToQuaternion(bone.morphedWorldOrientation, Quaternion2Angles.RotationOrder.ZYX);

            if (!bvhUseUnorientedSkeleton.val)
                copyDict[bone].transform.rotation = qf;
            else
                copyDict[bone].transform.localRotation = Quaternion.identity;

            if (bone.name == "hip")
                copyDict[bone].transform.localPosition = bone.startingLocalPosition;

            foreach (DAZBone child in dazChildBones[bone])
                SetCopiesToRestPose(child, copyDict);
        }

        void AddBVHFrame()
        {
            foreach (var pair in bvhData)
            {
                var root = pair.Key;
                var data = pair.Value;
                bool first = true;

                foreach (var bone in data.orderedBones)
                {
                    if (!first)
                        data.bvhOutput.Append(" ");
                    first = false;
                    if (bone.name == "hip")
                    {
                        if (bvhStartsAtOrigin.val)
                        {
                            Vector3 positionWithoutOffset = bone.transform.localPosition - data.hipStartingPosition;
                            Vector3 position = new Vector3(positionWithoutOffset.x, bone.transform.localPosition.y, positionWithoutOffset.z);
                            data.bvhOutput.Append(Vector3String(position, false) + " ");
                        }
                        else
                            data.bvhOutput.Append(Vector3String(bone.transform.localPosition, false) + " ");
                    }

                    adjustedRestSkeleton[bone].transform.rotation = bone.transform.rotation; // bring child rotation ref into orientation so we can invert it
                    adjustedRestSkeleton[bone].transform.rotation = bone.transform.rotation * Quaternion.Inverse(rotationRefs[bone].rotation) * bone.transform.rotation;

                    Quaternion quat = adjustedRestSkeleton[bone].transform.localRotation;

                    if (bvhUseUnorientedSkeleton.val)
                        quat = bone.transform.localRotation;

                    Quaternion quatMirrored = new Quaternion(quat.x, -quat.y, -quat.z, quat.w);

                    Vector3 eulers = Mathf.Rad2Deg * Quaternion2Angles.GetAngles(quatMirrored, bone.rotationOrder);

                    data.bvhOutput.Append(Vector3String(eulers, true, bone.rotationOrder));
                }
                data.bvhOutput.Append("\n");
                data.frames++;
            }
        }

        void BeginBVHRender()
        {
            timestamps.Clear();
            bRendering = true;
            bRecordVideo = true;

            if (renderCamParentObj != null)
                EndVRPreview();

            oldCaptureFramerate = Time.captureFramerate;
            Time.captureFramerate = frameRateInt;

            oldTimeScale = Time.timeScale;

            if (baseFilename == "" || baseFilename == null)
            {
                baseFilename = GetFilename();
                generatedFilename = true;
            }
            else
                generatedFilename = false;

            myDirectory = SCREENSHOT_DIRECTORY;

            FileManagerSecure.CreateDirectory(myDirectory);

            if (muteAudioChooser.val && !bActuallyRecordAudio)
            {
                AudioSource[] myAudioSources = UnityEngine.Object.FindObjectsOfType<AudioSource>();
                // Note: dictionary doesn't seem to compile in VaM plugin
                audioVolumes = new List<float>();
                audioSources = new List<AudioSource>();
                foreach (var source in myAudioSources)
                {
                    audioSources.Add(source);
                    audioVolumes.Add(source.volume);
                    source.volume = 0f;
                }
            }

            SetupBVH();
        }

        void EndBVHRender()
        {
            if (!bRendering)
                return;

            timestamps.Clear();
            bRendering = false;

            WriteBVHs();

            dazChildBones.Clear();

            foreach (var copy in adjustedRestSkeleton)
                Destroy(copy.Value);
            foreach (var copy in endSites)
                Destroy(copy.Value.gameObject);
            foreach (var copy in rotationRefs)
                Destroy(copy.Value.gameObject);

            adjustedRestSkeleton.Clear();
            endSites.Clear();
            rotationRefs.Clear();

            Time.captureFramerate = oldCaptureFramerate;

            if (unfreezeOnPlaybackChooser.val && unfrozeOnStart)
            {
                SuperController.singleton.SetFreezeAnimation(true);
                unfrozeOnStart = false;
            }

            Time.timeScale = oldTimeScale;

            if (muteAudioChooser.val && !bActuallyRecordAudio)
            {
                for (int i = 0; i < audioVolumes.Count; i++)
                    audioSources[i].volume = audioVolumes[i];
                audioVolumes.Clear();
                audioSources.Clear();
            }

            myNeedSetup = true;

            lastFilename = baseFilename;

            if (generatedFilename)
                baseFilename = "";

            SuperController.singleton.motionAnimationMaster.StopPlayback();

            renderEndedLastFrame = true;
        }

        void BeginRender()
        {
            if (bBvhRender)
            {
                BeginBVHRender();
                return;
            }

            timestamps.Clear();
            bRendering = true;
            if (!bFlatRender)
            {
                if (renderCamParentObj != null)
                    EndVRPreview();
            }

            if (bRecordVideo)
            {
                oldCaptureFramerate = Time.captureFramerate;
                Time.captureFramerate = frameRateInt;

                if (videoPlayer != null && renderBackgroundChooser.val)
                {
                    lastFrameReceived = -1;

                    if (!videoPlayer.canStep)
                    {
                        SuperController.LogMessage("Cannot step through this video to render - aborted.");
                        return;
                    }

                    videoTimeOnRenderBegin = (float)videoPlayer.time;
                    videoFrameReady = true;

                    if (videoPlayer.isPlaying)
                    {
                        videoPlayerWasPlaying = true;
                        shouldAdvanceVideo = true;
                        videoPlayer.Pause();
                    }
                    else
                    {
                        videoPlayerWasPlaying = false;
                        if (unpauseVideoOnRenderChooser.val)
                            shouldAdvanceVideo = true;
                        else
                            shouldAdvanceVideo = false;
                    }
                }
            }

            oldTimeScale = Time.timeScale;

            int width = finalResolution.x;
            int height = finalResolution.y;

            TextureFormat textureFormat = (myFileFormat == FORMAT_JPG || !(preserveTransparencyChooser.val && myFileFormat == FORMAT_PNG)) ? TextureFormat.RGB24 : TextureFormat.ARGB32;
            finalOutputTexture = new Texture2D(width, height, textureFormat, false);

            mySelectedController = SuperController.singleton.GetSelectedController();
            SuperController.singleton.ClearSelection();

            if (!bRecordVideo || lastFilename == "" || lastFilename == null)
            {
                if (baseFilename == "" || baseFilename == null)
                {
                    baseFilename = GetFilename();
                    generatedFilename = true;
                }
                else
                    generatedFilename = false;
            }
            else
            {
                baseFilename = lastFilename;
            }

            myDirectory = SCREENSHOT_DIRECTORY;
            if (bRecordVideo)
                myDirectory += baseFilename + "/";

            FileManagerSecure.CreateDirectory(myDirectory);

            if (renderCamParentObj == null)
                renderCamParentObj = new GameObject();
            if (renderCam == null)
                renderCam = CreateFlatCamera(renderCamParentObj.transform);

            if (bRecordVideo && recordAudioChooser.val)
                StartAudioRecording();

            SyncPostProcessing(renderCam);
            SyncCommandBuffers(renderCam);

            SyncCameraTransform();

            if (muteAudioChooser.val && !bActuallyRecordAudio)
            {
                AudioSource[] myAudioSources = UnityEngine.Object.FindObjectsOfType<AudioSource>();
                // Note: dictionary doesn't seem to compile in VaM plugin
                audioVolumes = new List<float>();
                audioSources = new List<AudioSource>();
                foreach (var source in myAudioSources)
                {
                    audioSources.Add(source);
                    audioVolumes.Add(source.volume);
                    source.volume = 0f;
                }
            }

            if (bFlatRender)
            {
                flatRenderTex = GetRenderTexture(width * flatSupersampling, height * flatSupersampling, myMsaaLevel, true);
                if ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null)))
                {
                    flatRenderTexWhite = GetRenderTexture(width * flatSupersampling, height * flatSupersampling, myMsaaLevel, true);
                    flatRenderTexBlack = GetRenderTexture(width * flatSupersampling, height * flatSupersampling, myMsaaLevel, true);
                }

                if (flatSupersampling > 1)
                {
                    bool horizontal = flatRenderTex.width >= flatRenderTex.height;
                    passTexture = GetRenderTexture(width * (horizontal ? 1 : flatSupersampling), height * (horizontal ? flatSupersampling : 1), 1, false);
                    outputRenderTexture = GetRenderTexture(width, height, 1, false);
                }
            }
            else
            {
                BeginVRRender(width, height);
            }
        }

        void BeginVRRender(int width, int height)
        {
            if (_equirectMat == null || _equirectMatAlpha == null)
            {
                SuperController.LogError("LilyRender shader not loaded.");
                return;
            }

            int sideWidth = width;
            int sideHeight = height;

            if (bStereoRender)
            {
                if (b180Degrees)
                    sideWidth = width / 2;
                else
                    sideHeight = height / 2;
            }

            equirectL = GetRenderTexture(sideWidth, sideHeight, 1, false);
            if (bStereoRender)
                equirectR = GetRenderTexture(sideWidth, sideHeight, 1, false);

            int sideCount = 6;

            if (bStereoRender)
            {
                if (stereoMode == STEREO_STATIC)
                    sideCount = 6;
                else if (stereoMode == STEREO_SQUARE)
                    sideCount = 12;
                else if (stereoMode == STEREO_TRIANGLE)
                    sideCount = 9;
                else if (stereoMode == STEREO_PANORAMIC)
                {
                    PreparePanoramicRender(sideWidth, panoramicVerticalFov);
                    return;
                }
            }

            if (stereoMode == STEREO_STATIC && ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null))))
            {
                blackBgRenderTex = GetRenderTexture(cubemapSideLength, cubemapSideLength, myMsaaLevel, true);
                whiteBgRenderTex = GetRenderTexture(cubemapSideLength, cubemapSideLength, myMsaaLevel, true);
            }

            renderFaces = new RenderTexture[sideCount];

            for (int i = 0; i < sideCount; i++)
            {
                if (bStereoRender && stereoMode == STEREO_SQUARE && i % 3 != 1)
                {
                    // bottom/top face
                    renderFaces[i] = GetRenderTexture(cubemapSideLength / 2, cubemapSideLength / 2, myMsaaLevel, true);
                }
                else
                {
                    renderFaces[i] = GetRenderTexture(cubemapSideLength, cubemapSideLength, myMsaaLevel, true);
                }
            }
        }

        void EndRender()
        {
            if (!bRendering)
                return;

            if (bBvhRender)
            {
                EndBVHRender();
                return;
            }

            timestamps.Clear();
            bRendering = false;

            if (bRecordVideo)
            {
                if (recordAudioChooser.val && bActuallyRecordAudio)
                    EndAudioRecording();

                Time.captureFramerate = oldCaptureFramerate;
                if (videoPlayer != null && renderBackgroundChooser.val)
                {
                    videoFrameReady = true;
                    if (videoPlayerWasPlaying)
                        videoPlayer.Play();
                    else
                        videoPlayer.Pause();
                    timeChooser.val = (float)videoPlayer.time;
                }
            }

            if (unfreezeOnPlaybackChooser.val && unfrozeOnStart)
            {
                SuperController.singleton.SetFreezeAnimation(true);
                unfrozeOnStart = false;
            }

            Destroy(renderCam);
            renderCam = null;

            Time.timeScale = oldTimeScale;

            if (muteAudioChooser.val && !bActuallyRecordAudio)
            {
                for (int i = 0; i < audioVolumes.Count; i++)
                    audioSources[i].volume = audioVolumes[i];
                audioVolumes.Clear();
                audioSources.Clear();
            }

            Destroy(finalOutputTexture);
            renderCamParentObj.SetActive(false);

            myNeedSetup = true;

            foreach (Transform c in renderCamParentObj.transform)
                Destroy(c);

            Destroy(renderCamParentObj);
            renderCamParentObj = null;

            lastFilename = baseFilename;

            if (generatedFilename)
                baseFilename = "";

            if (mySelectedController != null)
                SuperController.singleton.SelectController(mySelectedController);
            mySelectedController = null;
            if (bRecordVideo)
                SuperController.singleton.motionAnimationMaster.StopPlayback();

            if (bFlatRender)
            {
                Destroy(flatRenderTex);
                flatRenderTex = null;
                if ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null)))
                {
                    Destroy(flatRenderTexBlack);
                    flatRenderTexBlack = null;
                    Destroy(flatRenderTexWhite);
                    flatRenderTexWhite = null;
                }

                if (flatSupersampling > 1)
                {
                    Destroy(passTexture);
                    Destroy(outputRenderTexture);
                }
            }
            else
            {
                foreach (var rt in renderFaces)
                    Destroy(rt);
                Destroy(equirectL);
                if (stereoMode == STEREO_STATIC && ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null))))
                {
                    Destroy(blackBgRenderTex);
                    blackBgRenderTex = null;
                    Destroy(whiteBgRenderTex);
                    whiteBgRenderTex = null;
                }
            }

            if (!bFlatRender && previewChooser.val)
            {
                renderEndedLastFrame = true;
                //   BeginVRPreview();
            }
        }

        private Camera CreateFlatCamera(Transform parent)
        {
            Camera screenshotCamera = SuperController.singleton.hiResScreenshotCamera;

            int cullingMask = 1 << LayerMask.NameToLayer("UI")
                            | 1 << LayerMask.NameToLayer("LoadUI")
                            | 1 << LayerMask.NameToLayer("ScreenUI")
                            | 1 << LayerMask.NameToLayer("GUI");
            cullingMask = screenshotCamera.cullingMask & ~cullingMask;

            Camera c = Instantiate<Camera>(screenshotCamera, parent);

            MoveAndRotateAs mr = c.GetComponent<MoveAndRotateAs>();
            if (mr)
                mr.MoveAndRotateAsTransform = null;
            c.gameObject.name = "SubCamera";
            c.enabled = true;

            c.transform.localPosition = Vector3.zero;
            c.transform.localRotation = Quaternion.identity;

            c.fieldOfView = 90f;
            c.cullingMask = cullingMask;
            c.targetTexture = null;
            c.clearFlags = CameraClearFlags.SolidColor;
            c.backgroundColor = Color.black;
            c.enabled = false;

            return c;
        }

        Vector3[] panoramaPositionsL;
        Vector3[] panoramaPositionsR;
        Quaternion[] panoramaRotations;

        void PreparePanoramicRender(int eyeWidth, float verticalFov)
        {
            panoramaPositionsL = new Vector3[eyeWidth * 3];
            panoramaPositionsR = new Vector3[eyeWidth * 3];
            panoramaRotations = new Quaternion[eyeWidth * 3];

            Material renderMat = sliceEquirectMat;

            renderMat.SetFloat("_HideSize", hideSizeChooser.val / 180f);
            renderMat.SetFloat("_VerticalFov", verticalFov);

            float horizontalFov = b180Degrees ? 180f : 360f;

            Vector3 pivot = Vector3.back * (eyePivotDistanceChooser.val / 1000f);

            for (int i = 0; i < eyeWidth; i++)
            {
                for (int side = 0; side < 2; side++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        Quaternion horizontalRotation = Quaternion.AngleAxis(-horizontalFov / 2 + i * horizontalFov / (float)eyeWidth, Vector3.up);
                        panoramaRotations[i * 3 + y] = horizontalRotation;
                        panoramaRotations[i * 3 + y] *= Quaternion.AngleAxis(verticalFov - y * verticalFov, Vector3.right);
                        if (side == 0)
                            panoramaPositionsL[i * 3 + y] = horizontalRotation * (((ipd / 2000f) * Vector3.left) - pivot) + pivot;
                        else
                            panoramaPositionsR[i * 3 + y] = horizontalRotation * (((ipd / 2000f) * Vector3.right) - pivot) + pivot;
                    }
                }
            }
        }

        void RenderPanoramicStereoMap(Camera cam, Texture2D outputTexture, Vector3 sideVector, float horizontalFov)
        {
            bool bPreserveAlpha = (myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null));

            float verticalFov = panoramicVerticalFov;
            int eyeWidth = b180Degrees ? outputTexture.width / 2 : outputTexture.width;
            int eyeHeight = b180Degrees ? outputTexture.height : outputTexture.height / 2;

            RenderTexture prev = RenderTexture.active;
            cam.fieldOfView = verticalFov;

            RenderTexture bufferTex = GetRenderTexture(eyeWidth, eyeHeight, 1, false);
            RenderTexture textureBlack = GetRenderTexture(1, cubemapSideLength, myMsaaLevel, true);
            RenderTexture textureWhite = GetRenderTexture(1, cubemapSideLength, myMsaaLevel, true);

            RenderTexture[] textures = new RenderTexture[3];
            for (int i = 0; i < 3; i++)
                textures[i] = GetRenderTexture(1, cubemapSideLength, myMsaaLevel, true);

            sliceEquirectMat.SetFloat("_VerticalFov", Mathf.Deg2Rad * verticalFov);

            Color customBackgroundColor = bPreserveAlpha ? Color.clear : Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);

            if (!bPreserveAlpha)
                cam.backgroundColor = customBackgroundColor;

            Texture background = GetCurrentBackground();

            for (int side = 0; side < 2; side++)
            {
                Vector3[] panoramaPositions = side == 0 ? panoramaPositionsL : panoramaPositionsR;
                RenderTexture target = side == 0 ? equirectL : equirectR;
                Graphics.Blit(clearTex, bufferTex);
                for (int i = 0; i < eyeWidth; i++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cam.transform.localPosition = panoramaPositions[i * 3 + y];
                        cam.transform.localRotation = panoramaRotations[i * 3 + y];
                        if (bPreserveAlpha)
                        {
                            cam.backgroundColor = Color.black;
                            cam.targetTexture = textureBlack;
                            cam.Render();
                            cam.backgroundColor = Color.white;
                            cam.targetTexture = textureWhite;
                            cam.Render();
                            AlphaFromDifferenceShader(textureBlack, textureWhite, textures[y]);
                        }
                        else
                        {
                            cam.targetTexture = textures[y];
                            cam.Render();
                        }
                    }
                    PixelSliceEquirectShader(textures, target, bufferTex, i, background, side == 0);
                    Graphics.Blit(target, bufferTex);
                }
            }

            Rect fullRect = new Rect(0, 0, eyeWidth, eyeHeight);
            if (b180Degrees)
            {
                RenderTexture.active = equirectL;
                outputTexture.ReadPixels(fullRect, 0, 0, false);
                RenderTexture.active = equirectR;
                outputTexture.ReadPixels(fullRect, eyeWidth, 0, false);
            }
            else
            {
                RenderTexture.active = equirectR;
                outputTexture.ReadPixels(fullRect, 0, 0, false);
                RenderTexture.active = equirectL;
                outputTexture.ReadPixels(fullRect, 0, eyeHeight, false);
            }

            Destroy(textureBlack);
            Destroy(textureWhite);
            Destroy(bufferTex);
            foreach (RenderTexture rt in textures)
                Destroy(rt);

            RenderTexture.active = prev;
        }

        void PixelSliceEquirectShader(RenderTexture[] sliceTextures, RenderTexture outputTexture, RenderTexture sourceTex, int pixelColumn, Texture background = null, bool bLeftSide = true)
        {
            Material renderMat = sliceEquirectMat;
            bool wholeImage = true;
            if (wholeImage)
            {
                renderMat.EnableKeyword("WHOLE_TEXTURE");
                renderMat.SetFloat("_PixelColumn", (float)pixelColumn / (float)outputTexture.width);
                renderMat.SetTexture("_MainTex", sourceTex);

                SetBackgroundParameters(renderMat, background, bLeftSide);
            }
            else
                renderMat.DisableKeyword("WHOLE_TEXTURE");

            renderMat.SetTexture("_BotTex", sliceTextures[0]);
            renderMat.SetTexture("_MidTex", sliceTextures[1]);
            renderMat.SetTexture("_TopTex", sliceTextures[2]);


            Graphics.Blit(null, outputTexture, renderMat, 0);
        }

        void RenderTriangleMap(Camera cam, RenderTexture[] textures, Vector3 sideVector)
        {
            for (int hor = 0; hor < 3; hor++)
            {
                for (int ver = 0; ver < 3; ver++)
                {
                    float sideFov = ((360 - frontFovChooser.val) / 2);
                    float verticalFov = hor == 0 ? frontFovChooser.val : sideFov;

                    float rotation = hor == 0 ? 0 : (frontFovChooser.val + sideFov) / 2;
                    rotation *= hor == 1 ? 1 : -1;

                    cam.targetTexture = textures[hor * 3 + ver];
                    Quaternion horizontalRotation = Quaternion.AngleAxis(rotation, Vector3.up);
                    cam.transform.localRotation = horizontalRotation;
                    cam.transform.localRotation = cam.transform.localRotation * Quaternion.AngleAxis(90 - 90f * ver, Vector3.right);
                    cam.transform.localPosition = sideVector;
                    cam.fieldOfView = verticalFov;

                    Vector3 pivot = Vector3.back * (eyePivotDistanceChooser.val / 1000f);
                    cam.transform.localPosition = horizontalRotation * (cam.transform.localPosition - pivot) + pivot;

                    cam.Render();
                }
            }
        }

        void RenderSquareMap(Camera cam, RenderTexture[] textures, Vector3 sideVector)
        {
            float verticalFov = 90f;

            if (lilyRenderOverlap != 0f)
            {
                verticalFov = 2f * Mathf.Atan(1f + lilyRenderOverlap) / Mathf.PI * 180;
            }

            for (int hor = 0; hor < 4; hor++)
            {
                if (b180Degrees && hor == 2)
                    continue;

                for (int ver = 0; ver < 3; ver++)
                {
                    cam.targetTexture = textures[hor * 3 + ver];
                    cam.fieldOfView = verticalFov;
                    Quaternion horizontalRotation = Quaternion.AngleAxis(hor * 90, Vector3.up);
                    cam.transform.localRotation = ver == 1 ? horizontalRotation : Quaternion.identity; // shader expects top and bottom renders to all face forward
                    cam.transform.localRotation = cam.transform.localRotation * Quaternion.AngleAxis(90 - 90f * ver, Vector3.right);
                    cam.transform.localPosition = sideVector;

                    Vector3 pivot = Vector3.back * (eyePivotDistanceChooser.val / 1000f);
                    cam.transform.localPosition = horizontalRotation * (cam.transform.localPosition - pivot) + pivot;

                    cam.Render();
                }
            }
        }

        private void RenderCubemap(Camera cam, RenderTexture[] textures, Vector3 sideVector)
        {
            bool bPreserveAlphaInRender = (myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) || (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null));
            bool bPreserveAlphaNow = bPreserveAlphaInRender || (!bRendering && hideBackgroundColorOnPreviewOnly.val);

            Quaternion[] rotations = new Quaternion[6] { // clockwise rotation when looking through the axis = negative value
            Quaternion.Euler( 90,0,0), // bottom
            Quaternion.Euler(270,0,0), // top
            Quaternion.Euler(0,180,0), // back
            Quaternion.Euler(0, 0, 0), // front
            Quaternion.Euler(0, 90,0), // right
            Quaternion.Euler(0, 270,0), // left
        };
            float verticalFov = 90f;

            if (!bPreserveAlphaNow)
                cam.backgroundColor = Color.HSVToRGB(backgroundColor.val.H, backgroundColor.val.S, backgroundColor.val.V);

            if (lilyRenderOverlap != 0f)
            {
                verticalFov = 2f * Mathf.Atan(1f + lilyRenderOverlap) / Mathf.PI * 180f;
            }

            for (int i = 0; i < 6; i++)
            {
                if (b180Degrees && i == 2)
                    continue;

                cam.transform.localPosition = sideVector;
                cam.transform.localRotation = rotations[i];
                cam.targetTexture = textures[i];
                cam.fieldOfView = verticalFov;

                if (!bPreserveAlphaNow)
                    cam.Render();
                else
                {
                    cam.targetTexture = blackBgRenderTex;
                    cam.backgroundColor = Color.black;
                    cam.Render();
                    cam.targetTexture = whiteBgRenderTex;
                    cam.backgroundColor = Color.white;
                    cam.Render();

                    AlphaFromDifferenceShader(blackBgRenderTex, whiteBgRenderTex, textures[i]);
                }
            }
        }

        void SetBackgroundParameters(Material renderMat, Texture background, bool bLeftSide)
        {
            if (background != null)
            {
                renderMat.EnableKeyword("USE_BACKGROUND");
                renderMat.SetTexture("_BackgroundTex", background);
                renderMat.SetTexture("_Background", background);

                if (backgroundStereoLayout == BG_MONO)
                    renderMat.SetFloat("_EyeLocation", 0);
                else
                {
                    if (backgroundStereoLayout == BG_LEFTRIGHT)
                        renderMat.SetFloat("_EyeLocation", bLeftSide ? 1 : 2);
                    else if (backgroundStereoLayout == BG_RIGHTLEFT)
                        renderMat.SetFloat("_EyeLocation", bLeftSide ? 2 : 1);
                    else if (backgroundStereoLayout == BG_DOWNUP)
                        renderMat.SetFloat("_EyeLocation", bLeftSide ? 3 : 4);
                    else if (backgroundStereoLayout == BG_UPDOWN)
                        renderMat.SetFloat("_EyeLocation", bLeftSide ? 4 : 3);
                }
            }
            else
                renderMat.DisableKeyword("USE_BACKGROUND");
        }

        void AlphaFromDifferenceShader(RenderTexture blackBgTex, RenderTexture whiteBgTex, RenderTexture outputTexture, Texture background = null, bool? customLeftSide = null)
        {
            Material renderMat = alphaDiffMat;

            renderMat.SetTexture("_BlackBgTex", blackBgTex);
            renderMat.SetTexture("_WhiteBgTex", whiteBgTex);

            bool bLeftSide = (outputTexture == equirectL);
            if (customLeftSide != null)
                bLeftSide = (bool)customLeftSide;

            SetBackgroundParameters(renderMat, background, bLeftSide);

            Graphics.Blit(null, outputTexture, renderMat);
        }

        /*
        Color[] debugColors;
        Texture2D[] debugTex;
        RenderTexture[] debugTexRT;

        void CreateDebugTex()
        {
            debugTex = new Texture2D[12];
            debugTexRT = new RenderTexture[12];
            debugColors = new Color[] { Color.white, Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan, new Color(0.25f, 0.5f, 0.75f, 1f), Color.grey, new Color(1, 0.5f, 0.5f, 1), new Color(0.5f, 1, 0.5f, 1), new Color(0.5f, 0.5f, 1, 1) };
            for (int i = 0; i < debugTex.Length; i++)
            {
                debugTex[i] = new Texture2D(1, 1);
                debugTex[i].SetPixel(0, 0, debugColors[i]);
                debugTex[i].Apply();

                debugTexRT[i] = GetRenderTexture(1, 1, 1, false);
                Graphics.Blit(debugTex[i], debugTexRT[i]);
            }
        }
        */

        void TriangleMapToEquirectShader(RenderTexture[] faces, RenderTexture outputTexture)
        {
            Material renderMat = _equirectMatRotateTriangle;

            Matrix4x4 orientMatrix = Matrix4x4.identity;

            renderMat.SetTexture("_FaceABot", faces[0]);
            renderMat.SetTexture("_FaceAMid", faces[1]);
            renderMat.SetTexture("_FaceATop", faces[2]);
            renderMat.SetTexture("_FaceBBot", faces[3]);
            renderMat.SetTexture("_FaceBMid", faces[4]);
            renderMat.SetTexture("_FaceBTop", faces[5]);
            renderMat.SetTexture("_FaceCBot", faces[6]);
            renderMat.SetTexture("_FaceCMid", faces[7]);
            renderMat.SetTexture("_FaceCTop", faces[8]);
            Color cc = Color.HSVToRGB(seamTextureTintChooser.val.H, seamTextureTintChooser.val.S, seamTextureTintChooser.val.V);
            renderMat.SetColor("_HideColor", cc);
            if (seamTexture != null)
            {
                renderMat.SetTexture("_SeamTex", seamTexture);
                renderMat.SetFloat("_UseSeamTex", 1);
            }
            else
            {
                renderMat.SetFloat("_UseSeamTex", 0);
            }
            renderMat.SetFloat("_FrontFov", frontFovChooser.val * Mathf.Deg2Rad);
            renderMat.SetFloat("_HideSize", hideSizeChooser.val / 180f);
            renderMat.SetFloat("_HideSeamsSize", hideSeamsSizeChooser.val / 100);
            renderMat.SetFloat("_Offset", hideSeamsParallaxChooser.val / 100f);
            renderMat.SetFloat("_EyeInfo", outputTexture == equirectL ? -1 : 1);

            if (lilyRenderOverlap > 0f)
                renderMat.EnableKeyword("SMOOTH_STITCHING");
            else
                renderMat.DisableKeyword("SMOOTH_STITCHING");

            renderMat.SetMatrix("_OrientMatrix", orientMatrix);
            renderMat.SetFloat("_Beta", 1f / (1f + lilyRenderOverlap));

            float horizontalFov = (b180Degrees) ? 180f : 360f;
            renderMat.SetFloat("_HorizontalFov", horizontalFov * Mathf.Deg2Rad);
            renderMat.SetFloat("_VerticalFov", 180f * Mathf.Deg2Rad);

            Graphics.Blit(null, outputTexture, renderMat, 0);
        }

        void SquareMapToEquirectShader(RenderTexture[] faces, RenderTexture outputTexture)
        {
            Material renderMat = _equirectMatRotate;

            Matrix4x4 orientMatrix = Matrix4x4.identity;

            renderMat.SetTexture("_FaceTexPYA", faces[0]); // front bottom
            renderMat.SetTexture("_FaceTexPZ", faces[1]); // front mid
            renderMat.SetTexture("_FaceTexNYA", faces[2]); // front top

            renderMat.SetTexture("_FaceTexPYB", faces[3]); // right bottom
            renderMat.SetTexture("_FaceTexPX", faces[4]); // right mid
            renderMat.SetTexture("_FaceTexNYB", faces[5]); // right top

            renderMat.SetTexture("_FaceTexPYC", faces[6]); // back bottom
            renderMat.SetTexture("_FaceTexNZ", faces[7]); // back mid
            renderMat.SetTexture("_FaceTexNYC", faces[8]); // back top

            renderMat.SetTexture("_FaceTexPYD", faces[9]); // left bottom
            renderMat.SetTexture("_FaceTexNX", faces[10]); // left mid
            renderMat.SetTexture("_FaceTexNYD", faces[11]); // left top

            if (seamTexture != null)
            {
                renderMat.SetTexture("_SeamTex", seamTexture);
                renderMat.SetFloat("_UseSeamTex", 1);
            }
            else
            {
                renderMat.SetFloat("_UseSeamTex", 0);
            }

            renderMat.SetFloat("_HideSize", hideSizeChooser.val / 180f);
            renderMat.SetFloat("_HideSeamsSize", hideSeamsSizeChooser.val / 100);
            renderMat.SetFloat("_Offset", hideSeamsParallaxChooser.val / 100f);
            renderMat.SetFloat("_EyeInfo", outputTexture == equirectL ? -1 : 1);
            Color cc = Color.HSVToRGB(seamTextureTintChooser.val.H, seamTextureTintChooser.val.S, seamTextureTintChooser.val.V);
            renderMat.SetColor("_HideColor", cc);

            if (lilyRenderOverlap > 0f)
                renderMat.EnableKeyword("SMOOTH_STITCHING");
            else
                renderMat.DisableKeyword("SMOOTH_STITCHING");

            renderMat.SetMatrix("_OrientMatrix", orientMatrix);
            renderMat.SetFloat("_Beta", 1f / (1f + lilyRenderOverlap));

            float horizontalFov = (b180Degrees) ? 180f : 360f;
            renderMat.SetFloat("_HorizontalFov", horizontalFov * Mathf.Deg2Rad);
            renderMat.SetFloat("_VerticalFov", 180f * Mathf.Deg2Rad);

            renderMat.SetTexture("_FaceTexPY", blackTex);
            renderMat.SetTexture("_FaceTexNY", blackTex);

            Graphics.Blit(null, outputTexture, renderMat, 0);
        }

        void CubemapToEquirectShader(RenderTexture[] faces, RenderTexture outputTexture, Texture background = null)
        {
            Material renderMat;
            if ((myFileFormat == FORMAT_PNG && preserveTransparencyChooser.val) ||
                (renderBackgroundChooser.val && (previewBackground != null || videoPlayer != null)) ||
                !bRendering)
                renderMat = _equirectMatAlpha;
            else
                renderMat = _equirectMat;

            Matrix4x4 orientMatrix = Matrix4x4.identity;

            renderMat.SetTexture("_FaceTexPY", faces[0]); // bottom
            renderMat.SetTexture("_FaceTexNY", faces[1]); // top
            renderMat.SetTexture("_FaceTexPZ", faces[3]); // front
            renderMat.SetTexture("_FaceTexNZ", faces[2]); // back
            renderMat.SetTexture("_FaceTexPX", faces[4]); // right
            renderMat.SetTexture("_FaceTexNX", faces[5]); // left

            if (lilyRenderOverlap > 0f)
                renderMat.EnableKeyword("SMOOTH_STITCHING");
            else
                renderMat.DisableKeyword("SMOOTH_STITCHING");

            renderMat.SetMatrix("_OrientMatrix", orientMatrix);
            renderMat.SetFloat("_Beta", 1f / (1f + lilyRenderOverlap));

            float horizontalFov = (b180Degrees) ? 180f : 360f;
            renderMat.SetFloat("_HorizontalFov", horizontalFov * Mathf.Deg2Rad);
            renderMat.SetFloat("_VerticalFov", 180f * Mathf.Deg2Rad);

            bool bLeftSide = (outputTexture == equirectL);

            if (background != null)
                SetBackgroundParameters(renderMat, background, bLeftSide);

            Graphics.Blit(null, outputTexture, renderMat, 0);
        }
    }
}