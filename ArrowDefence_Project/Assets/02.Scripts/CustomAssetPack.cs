namespace ActionCat {
#if UNITY_EDITOR
    using UnityEngine;
    using UnityEditor;
    using Google.Android.AppBundle.Editor;
    using Google.Android.AppBundle.Editor.AssetPacks;

#endif

    public static class CustomAssetPack {
        public static string SoundAssetPackName = "sound_assetpack";
        public static string FontsAssetPackName = "fonts_assetpack";
        public static string SoundAssetFolderPath = "Assets/10.Sounds/packing_sound";
        public static string FontsAssetFolderPath = "Assets/04.Fonts/packing_fonts";
#if UNITY_EDITOR
        [MenuItem("ActionCat/Asset Packing")]
        public static void ConfigureAssetPacks() {
            var assetPackConfig = new AssetPackConfig();
            assetPackConfig.AddAssetsFolder(SoundAssetPackName, SoundAssetFolderPath, AssetPackDeliveryMode.FastFollow);
            assetPackConfig.AddAssetsFolder(FontsAssetPackName, FontsAssetFolderPath, AssetPackDeliveryMode.FastFollow);
            AssetPackConfigSerializer.SaveConfig(assetPackConfig);

            //assetPackConfig.SplitBaseModuleAssets = true; // << 쓸거면 SaveConfig 위로 올려서 저장하기 !
            //SplitBaseModuleAssets이 무엇이냐 - 
            //AAB의 기본 모듈(빌드에 포함되는)에 있는 자산이 별도의 설치시간 자산을 분할되어야 하는 AssetPack임을 나타냄
            //일반적으로 게임을 빌드할 때 일부 자산은 기본 모듈의 "Assets" 디렉토리에 저장되는데, APK 크기 제한이 있는 게임은
            //이전 세대에는 "Split Application Binary"를 사용했을거임
            //APK확장 (*.obb) 파일을 생성하는 옵션이 있지만 지금의 .AAB (안드로이드 앱 번들)에서는 지원되지 않음.
            //만약 빌드하고 나서 Play Console에서 기본 모듈이 다운로드 크기 제한 (기준:150nb)를 초과해서 업로드 하지 못한다는 경고가
            //뜨는 경우에 이 옵션을 활성화해주면 문제가 해결될 수 있다고 함.
        }
#endif
    }
}
