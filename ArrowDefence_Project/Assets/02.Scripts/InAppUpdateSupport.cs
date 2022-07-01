namespace ActionCat {
    using Google.Play.AppUpdate;
    using Google.Play.Common;
    using System.Collections;
    using UnityEngine;

    public class InAppUpdateSupport {
        AppUpdateManager appUpdateManager = null;
        AppUpdateInfo appUpdateInfoResult = null;

        public void Init() {
            appUpdateManager = new AppUpdateManager();
        }

        public IEnumerator CheckForUpdate() {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

            // Wait Until the Asynchronous Operation Completes.
            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful) {
                //var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
                // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
                // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
                // to start an in-app update.

                appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            }
            else {
                // Log appUpdateInfoOperation.Error.
                switch (appUpdateInfoOperation.Error) {
                    case AppUpdateErrorCode.NoError:                 break;
                    case AppUpdateErrorCode.NoErrorPartiallyAllowed: break;
                    case AppUpdateErrorCode.ErrorUnknown:            break;
                    case AppUpdateErrorCode.ErrorApiNotAvailable:    break;
                    case AppUpdateErrorCode.ErrorInvalidRequest:     break;
                    case AppUpdateErrorCode.ErrorUpdateUnavailable:  break;
                    case AppUpdateErrorCode.ErrorUpdateNotAllowed:   break;
                    case AppUpdateErrorCode.ErrorDownloadNotPresent: break;
                    case AppUpdateErrorCode.ErrorUpdateInProgress:   break;
                    case AppUpdateErrorCode.ErrorInternalError:      break;
                    case AppUpdateErrorCode.ErrorUserCanceled:       break;
                    case AppUpdateErrorCode.ErrorUpdateFailed:       break;
                    case AppUpdateErrorCode.ErrorPlayStoreNotFound:  break;
                    case AppUpdateErrorCode.ErrorAppNotOwned:        break;
                    default:                                         break;
                }

                CatLog.ELog("업데이트의 정보를 취득하는데 에러가 발생했습니다. 업데이트 정보를 취득하지 못했습니다.");
            }
        }

        public bool IsUpdateAvailable() {
            if (appUpdateInfoResult == null) {
                throw new System.Exception("먼저 CheckForUpdate를 사용가능한 업데이트가 있는지에 대한 정보를 취득하세요.");
            }

            switch (appUpdateInfoResult.UpdateAvailability) {
                case UpdateAvailability.Unknown:                            CatLog.Log("In-App Update Info State: {Unknown}.                            result: false."); return false;
                case UpdateAvailability.UpdateNotAvailable:                 CatLog.Log("In-App Update Info State: {UpdateNotAvailable}.                 result: false."); return false;
                case UpdateAvailability.UpdateAvailable:                    CatLog.Log("In-App Update Info State: {UpdateAvailable}.                    result: true.");  return true;
                case UpdateAvailability.DeveloperTriggeredUpdateInProgress: CatLog.Log("In-App Update Info State: {DeveloperTriggeredUpdateInProgress}. result: false."); return false;
                default:                                                    CatLog.Log("In-App Update Info State: {defaultInSwitch}.                    result: false."); return false;
            }
        }

        public IEnumerator StartImmediateUpdate(System.Action failedOrDeniedCallback = null) {
            var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions(allowAssetPackDeletion: false);
            
            //이 업데이트 옵션이 유효한지 확인
            if (appUpdateInfoResult.IsUpdateTypeAllowed(appUpdateOptions) == false) {
                CatLog.ELog("해당 옵션의 앱 업데이트를 지원하지 않습니다.");
                yield break; //업데이트 중지
            }
            
            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
            // The result returned by PlayAsyncOperation.GetResult().
            // The AppUpdateOptions created defining the requested in-app update
            // and its parameters.

            yield return startUpdateRequest;

            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (for
            // example, by logging result.Error or by displaying a message to the user).
            switch (startUpdateRequest.Error) {
                case AppUpdateErrorCode.NoError:                 break;
                case AppUpdateErrorCode.NoErrorPartiallyAllowed: break;
                case AppUpdateErrorCode.ErrorUnknown:            break;
                case AppUpdateErrorCode.ErrorApiNotAvailable:    break;
                case AppUpdateErrorCode.ErrorInvalidRequest:     break;
                case AppUpdateErrorCode.ErrorUpdateUnavailable:  break;
                case AppUpdateErrorCode.ErrorUpdateNotAllowed:   break;
                case AppUpdateErrorCode.ErrorDownloadNotPresent: break;
                case AppUpdateErrorCode.ErrorUpdateInProgress:   break;
                case AppUpdateErrorCode.ErrorInternalError:      break;
                case AppUpdateErrorCode.ErrorUserCanceled:       break;
                case AppUpdateErrorCode.ErrorUpdateFailed:       break;
                case AppUpdateErrorCode.ErrorPlayStoreNotFound:  break;
                case AppUpdateErrorCode.ErrorAppNotOwned:        break;
                default:                                         break;
            }

            failedOrDeniedCallback?.Invoke();

            CatLog.ELog("오류가 발생했거나, 사용자가 업데이트를 취소했습니다.");
        }
    }
}
