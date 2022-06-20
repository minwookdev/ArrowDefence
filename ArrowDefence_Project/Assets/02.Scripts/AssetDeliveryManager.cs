namespace ActionCat {
    using UnityEngine;
    using Google.Play.AssetDelivery;
    using System.Collections;

    public class AssetDeliveryManager {
        bool isSoundPackDownloaded = false;
        bool isFontsPackDownloaded = false;
        float currentDownloadProgress;
        long totalDownloadSize;

        public long TotalDownloadSize {
            get => totalDownloadSize;
        }

        public float CurrentDownloaded {
            get => currentDownloadProgress;
        }

        public bool IsSoundPackDownloaded {
            get => isSoundPackDownloaded;
        }

        public bool IsFontsPackDownloaded {
            get => isFontsPackDownloaded;
        }

        /// <summary>
        /// 모든 에셋팩이 설치가 되었는지 체크
        /// </summary>
        /// <returns></returns>
        public bool IsDownloadedAllAssetPacks() {
            isSoundPackDownloaded = PlayAssetDelivery.IsDownloaded(CustomAssetPack.SoundAssetPackName);
            isFontsPackDownloaded = PlayAssetDelivery.IsDownloaded(CustomAssetPack.FontsAssetPackName);
            return (isSoundPackDownloaded && isFontsPackDownloaded);
        }

        public IEnumerator LoadSoundAssetPackAsync() {
            PlayAssetPackRequest packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(CustomAssetPack.SoundAssetPackName);

            while (!packRequest.IsDone) {
                if (packRequest.Status == AssetDeliveryStatus.WaitingForWifi) {                       // Wifi가 연결되지않아 대기중인 경우
                    var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation(); // Wifi연결안되면 데이터쓸건데 괜찮냐고 물어보는것 같음..

                    // Wait for confirmation dialog action. 유저 선택-대기
                    yield return userConfirmationOperation;

                    if ((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) || (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted)) {
                        //The user did not accept the confirmation - handle as needed. 
                        //여기서 와이파이가 아닌 데이터 다운로드가 싫다고 유저가 선택한 경우니까 이 경우에 어떻게 진행할지는 직접 구현해줘야함.

                        //임시방편으로 일단 어플이 강제종료 되도록 해둠
                        Application.Quit();
                    }

                    // Wait for Wi-Fi connection OR confirmation dialog acceptance before moving on.
                    yield return new WaitUntil(() => packRequest.Status != AssetDeliveryStatus.WaitingForWifi);
                }

                // Update CurrentDownLoad Value
                currentDownloadProgress = packRequest.DownloadProgress;

                // Use bundleRequest.DownloadProgress to track download progress.
                // Use bundleRequest.Status to track the status of request.
                yield return null;  // IsDone 완료될 때 까지 대기
            }

            // 1f(100%) 에 도달하지 못하는 경우 방지
            currentDownloadProgress = packRequest.DownloadProgress;

            if (packRequest.Error != AssetDeliveryErrorCode.NoError) {
                // There was an error retrieving the bundle. For error codes NetworkError
                // and InsufficientStorage, you may prompt the user to check their
                // connection settings or check their storage space, respectively, then
                // try again.
                CatLog.ELog("An unknown error has occurred.");
                yield return null;
            }

            // Request was successful. Retrieve AssetBundle from request.AssetBundle.
            //AssetPack assetPack = packRequest.AssetBundle;
        }

        public IEnumerator LoadFontsAssetPackAsync() {
            PlayAssetPackRequest packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(CustomAssetPack.FontsAssetPackName);

            while (!packRequest.IsDone) {
                if (packRequest.Status == AssetDeliveryStatus.WaitingForWifi) {
                    var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation();

                    // Wait for Confirmation dialog action.
                    yield return userConfirmationOperation;

                    if ((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) || (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted)) {
                        //The user did not accept the confirmation - handle as needed. 
                        Application.Quit();
                    }

                    // Wait for Wi-Fi connection OR confirmation dialog acceptance before moving on.
                    yield return new WaitUntil(() => packRequest.Status != AssetDeliveryStatus.WaitingForWifi);
                }

                // Update CurrentDownLoad Value
                currentDownloadProgress = packRequest.DownloadProgress;

                // Use bundleRequest.DownloadProgress to track download progress.
                // Use bundleRequest.Status to track the status of request.
                yield return null;
            }

            // 1f(100%) 에 도달하지 못하는 경우 방지
            currentDownloadProgress = packRequest.DownloadProgress;

            if (packRequest.Error != AssetDeliveryErrorCode.NoError) {
                // There was an error retrieving the bundle. For error codes NetworkError
                // and InsufficientStorage, you may prompt the user to check their
                // connection settings or check their storage space, respectively, then
                // try again.
                CatLog.ELog("An unknown error has occurred.");
                yield return null;
            }

            //AssetPack assetPack = packRequest.
        }

        public IEnumerator GetTotalDownloadSizeAsync(string assetPackName) {
            totalDownloadSize = 0;
            var getDownloadSizeOperation = PlayAssetDelivery.GetDownloadSize(assetPackName);
            while (!getDownloadSizeOperation.IsDone) {
                if (getDownloadSizeOperation.Error != AssetDeliveryErrorCode.NoError) {
                    totalDownloadSize = 0;
                    CatLog.ELog($"Failed to Get AssetPack Download Size {assetPackName}");
                    yield break; //돌다가 에러나면 코루틴 중지 
                }

                yield return null;
            }

            if (getDownloadSizeOperation.IsSuccessful) {
                totalDownloadSize = getDownloadSizeOperation.GetResult();
            }
            else {
                totalDownloadSize = 0;
                CatLog.ELog($"Failed to Get AssetPack Download Size {assetPackName}");
            }
        }

        public string GetAssetPackDownloadSize() {
            return FormatBytes(totalDownloadSize);
        }

        /// <summary>
        /// 파일 용량 계산
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        string FormatBytes(long bytes) {
            int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Mathf.Pow(scale, orders.Length - 1);
            foreach (string order in orders) {
                if(bytes > max) {
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                }
                max /= scale;
            }
            return "0.00 Bytes";
        }

        /// <summary>
        /// 용량계산 함수 위에꺼랑 똑같음
        /// </summary>
        /// <param name="bytes">계산할 용량</param>
        /// <param name="pi">표시 할 소숫점 자릿 수</param>
        /// <returns></returns>
        string GetFileSize2Bytes(long bytes, int pi) {
            int mok = 0;
            double fileSize = bytes;
            string space = "";
            string returnStr = "";
            while (fileSize > 1024.0) {
                fileSize /= 1024.0;
                mok++;
            }

            if      (mok == 1) space = "KB";
            else if (mok == 2) space = "MB";
            else if (mok == 3) space = "GB";
            else if (mok == 4) space = "TB";

            if (mok != 0) {
                if      (pi == 1) returnStr = string.Format("{0:F1} {1}", fileSize, space);
                else if (pi == 2) returnStr = string.Format("{0:F2} {1}", fileSize, space);
                else if (pi == 3) returnStr = string.Format("{0:F3} {1}", fileSize, space);
                else              returnStr = string.Format("{0} {1}", System.Convert.ToInt32(fileSize), space);
            }
            else {
                returnStr = string.Format("{0} {1}", System.Convert.ToInt32(fileSize), space);
            }
            return returnStr;
        }
    }
}
