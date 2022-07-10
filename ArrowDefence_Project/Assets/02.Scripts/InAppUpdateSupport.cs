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
                string operationErrorLog = "";
                switch (appUpdateInfoOperation.Error) {
                    case AppUpdateErrorCode.NoError:                 operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.NoErrorPartiallyAllowed: operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUnknown:            operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorApiNotAvailable:    operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorInvalidRequest:     operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUpdateUnavailable:  operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUpdateNotAllowed:   operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorDownloadNotPresent: operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUpdateInProgress:   operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorInternalError:      operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUserCanceled:       operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorUpdateFailed:       operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorPlayStoreNotFound:  operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    case AppUpdateErrorCode.ErrorAppNotOwned:        operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                    default:                                         operationErrorLog = $"AppUpdateInfoOperation Error Log: {appUpdateInfoOperation.Error.ToString()}"; break;
                }

                CatLog.ELog($"업데이트의 정보를 취득하는데 오류가 발생했습니다. {operationErrorLog}");
            }
        }

        public bool IsUpdateAvailable() {
            if (appUpdateInfoResult == null) {
                throw new System.Exception("먼저 CheckForUpdate를 사용가능한 업데이트가 있는지에 대한 정보를 취득하세요.");
            }

            // 앱 업데이트 가능 반환 결과
            switch (appUpdateInfoResult.UpdateAvailability) {   // 앱 업데이트 확인 결과
                case UpdateAvailability.UpdateAvailable:                    CatLog.Log($"AppUpdate Availability State: {appUpdateInfoResult.ToString()}"); return true;
                case UpdateAvailability.Unknown:                            CatLog.Log($"AppUpdate Availability State: {appUpdateInfoResult.ToString()}"); return false;
                case UpdateAvailability.UpdateNotAvailable:                 CatLog.Log($"AppUpdate Availability State: {appUpdateInfoResult.ToString()}"); return false;
                case UpdateAvailability.DeveloperTriggeredUpdateInProgress: CatLog.Log($"AppUpdate Availability State: {appUpdateInfoResult.ToString()}"); return false;
                default:                                                    CatLog.Log($"AppUpdate Availability State: {appUpdateInfoResult.ToString()}"); return false;
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
            string updateRequestLog = "";
            switch (startUpdateRequest.Error) {
                case AppUpdateErrorCode.NoError:                 updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.NoErrorPartiallyAllowed: updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUnknown:            updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorApiNotAvailable:    updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorInvalidRequest:     updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUpdateUnavailable:  updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUpdateNotAllowed:   updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorDownloadNotPresent: updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUpdateInProgress:   updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorInternalError:      updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUserCanceled:       updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorUpdateFailed:       updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorPlayStoreNotFound:  updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                case AppUpdateErrorCode.ErrorAppNotOwned:        updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
                default:                                         updateRequestLog = $"AppUpdate Request Error Log: {startUpdateRequest.Error.ToString()}"; break;
            }

            failedOrDeniedCallback?.Invoke();
            CatLog.ELog($"업데이트 요청 에러가 발생했습니다. {updateRequestLog}");
        }
    }
}
