namespace ActionCat {
    using System.Collections;
    using UnityEngine;
    using Google.Play.AssetDelivery;
    using Google.Play.Common;

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

        //public bool IsEffectPackDownloaded {
        //    get; protected set;
        //}
        //
        //public bool IsTileMapPackDownloaded {
        //    get; protected set;
        //}

        #region CHECK-INSTALLED-ASSETPACK

        /// <summary>
        /// 모든 에셋팩이 설치가 되었는지 체크
        /// </summary>
        /// <returns></returns>
        public bool IsDownloadedAllAssetPacks() {
            isSoundPackDownloaded = PlayAssetDelivery.IsDownloaded(CustomAssetPack.SoundAssetPackName);
            isFontsPackDownloaded = PlayAssetDelivery.IsDownloaded(CustomAssetPack.FontsAssetPackName);
            return (isSoundPackDownloaded && isFontsPackDownloaded);
        }

        #endregion

        #region DOWNLOAD-ASSETPACK

        public IEnumerator LoadSoundAssetPackAsync() {
            PlayAssetPackRequest packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(CustomAssetPack.SoundAssetPackName);
            currentDownloadProgress = 0f;

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
            currentDownloadProgress = 0f;

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

        public IEnumerator LoadAssetPackAsync(string assetPackName) {
            if (CustomAssetPack.IsAssetPackNameCorrect(assetPackName) == false) { // 요청받은 에셋팩의 네임 체크
                CatLog.Log("요청한 에셋팩의 이름을 찾을 수 없습니다.");              // 요청 에셋팩의 이름이 잘못된 경우 다운로드 실패, 코루틴 탈출
                yield break;
            }

            PlayAssetPackRequest packRequest = PlayAssetDelivery.RetrieveAssetPackAsync(assetPackName);  // 에셋 다운로드 요청
            currentDownloadProgress = 0f;

            while (!packRequest.IsDone) {                                      // 다운로드 결과를 받을 수 있을때까지 while 루프
                if(packRequest.Status == AssetDeliveryStatus.WaitingForWifi) { // 다운로드 시 Wifi에 연결되어있지 않은 경우
                    // 셀룰러 데이터를 사용하여 다운로드를 진행 요청하는 팝업을 띄움
                    var userConfirmationOperation = PlayAssetDelivery.ShowCellularDataConfirmation();

                    // 유저선택 (셀룰러 데이터 사용/취소) 대기
                    yield return userConfirmationOperation;

                    if ((userConfirmationOperation.Error != AssetDeliveryErrorCode.NoError) || (userConfirmationOperation.GetResult() != ConfirmationDialogResult.Accepted)) {
                        // 유저가 다운로드를 선택하지 않은 경우
                        Application.Quit();
                    }

                    // Wifi에 연결되거나 셀룰러 데이터를 통한 다운로드를 승인 대기
                    yield return new WaitUntil(() => packRequest.Status != AssetDeliveryStatus.WaitingForWifi);
                }

                // 현재 다운로드 진행률 업데이트
                currentDownloadProgress = packRequest.DownloadProgress;
                yield return null;
            }

            // 다운로드 진행률이 1f(100%)에 도달하지 못하는 경우를 방지
            currentDownloadProgress = packRequest.DownloadProgress;

            // 다운로드 요청이 완료되었지만 오류가 발생한 경우: 
            if (packRequest.Error != AssetDeliveryErrorCode.NoError) {
                // 에셋 다운로드 요청지에서 최종적으로 모든 에셋팩의 정상 다운로드를 파악하고 조치. 에러 로그 띄워줌
                CatLog.ELog($"AssetPack Download Error: {packRequest.Error.ToString()}");
                yield return null;
            }
        }

        #endregion

        #region GET-DOWNLOAD-SIZE

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

        #endregion

        #region REMOVE-OLD-ASSETPACK

        /// <summary>
        /// 설치된 사용되지 않는 에셋 팩을 감지하고 지워야 할 에셋 팩의 이름을 반환합니다.
        /// </summary>
        /// <param name="detectedOldAssetPackNames"></param>
        /// <returns></returns>
        public bool IsDetectedOldAssetPack(out string[] detectedOldAssetPackNames) {
            bool isResult    = false;
            var oldPackNameList = new System.Collections.Generic.List<string>(CustomAssetPack.OldPackNames);
            for (int i = oldPackNameList.Count - 1; i >= 0; i--) {
                if (PlayAssetDelivery.IsDownloaded(oldPackNameList[i])) {
                    // 사용되지 않는 에셋 팩이 설치되어있음
                    isResult = true;
                }
                else {
                    // 설치되지 않은 경우 딜리트 타겟 리스트에서 지워주고 continue
                    oldPackNameList.Remove(oldPackNameList[i]);
                }
            }

            detectedOldAssetPackNames = oldPackNameList.ToArray();
            return isResult;
        }

        public IEnumerator RemoveOldAssetPacks(string[] detectedOldAssetPackNames) {
            foreach (var assetPackName in detectedOldAssetPackNames) {
                PlayAsyncOperation<VoidResult, AssetDeliveryErrorCode> removeOperation = PlayAssetDelivery.RemoveAssetPack(assetPackName);
                //removeOperation.Completed += (operation) => { } // 요런식으로 콜백으로 넣어줄 수 도 있음 !
                yield return removeOperation;
                switch (removeOperation.Error) {
                    case AssetDeliveryErrorCode.NoError:                 CatLog.Log($"사용되지 않는 에셋 팩: {assetPackName} 이 성공적으로 제거되었습니다."); break;
                    case AssetDeliveryErrorCode.AppUnavailable:          break;
                    case AssetDeliveryErrorCode.BundleUnavailable:       break;
                    case AssetDeliveryErrorCode.NetworkError:            break;
                    case AssetDeliveryErrorCode.AccessDenied:            break;
                    case AssetDeliveryErrorCode.InsufficientStorage:     break;
                    case AssetDeliveryErrorCode.AssetBundleLoadingError: break;
                    case AssetDeliveryErrorCode.Canceled:                break;
                    case AssetDeliveryErrorCode.InternalError:           break;
                    case AssetDeliveryErrorCode.PlayStoreNotFound:       break;
                    case AssetDeliveryErrorCode.NetworkUnrestricted:     break;
                    case AssetDeliveryErrorCode.AppNotOwned:             break;
                    default:                                             break;
                }
            }
        }

        #endregion

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
