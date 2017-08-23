namespace DotNet.Push.Core
{
    /// <summary>
    /// 다운스트림 HTTP 메시지(JSON)의 대상, 옵션, 페이로드
    /// </summary>
    public class IosPushProtocol
    {
        /// <summary>
        /// 이 매개변수는 메시지의 수신자를 지정합니다.
        /// 값은 등록 토큰, 알림 키 또는 주제여야 합니다.
        /// 여러 주제로 보내는 경우에는 이 필드를 설정해서는 안 됩니다.
        /// condition을 참조하세요.
        /// </summary>
        public string to
        {
            get; set;
        }

        /// <summary>
        /// 이 매개변수는 멀티캐스트 메시지를 수신하는 기기의 목록을 등록 토큰 또는 ID로 지정합니다. 
        /// 1~1,000개 사이의 등록 토큰이 포함되어야 합니다.
        /// 단일 수신자가 아닌 멀티캐스트 메시징의 경우에만 이 매개변수를 사용하세요. 
        /// 2개 이상의 등록 토큰으로 보내는 멀티캐스트 메시지는 HTTP JSON 형식만 사용할 수 있습니다.
        /// </summary>
        public string registration_ids
        {
            get; set;
        }

        /// <summary>
        /// 이 매개변수는 메시지 대상을 결정하는 조건의 논리식을 지정합니다.
        /// 지원되는 조건은 형식이 ''yourTopic' in topics'로 지정된 주제입니다.이 값은 대소문자를 구분합니다.
        /// 지원되는 연산자는 &&, ||입니다.주제 메시지당 최대 2개의 연산자가 지원됩니다.
        /// </summary>
        public string condition
        {
            get; set;
        }

        /// <summary>
        /// 이 매개변수는 전송을 재개할 수 있을 때 마지막 메시지만 보내도록 축소가 
        /// 가능한 메시지(예: collapse_key: "Updates Available" 포함 메시지) 그룹을 의미합니다. 
        /// 기기가 다시 온라인 또는 활성 상태가 되었을 때 동일한 메시지가 너무 많이 전송되는 것을 방지하기 위한 메시지입니다.
        /// 
        /// 메시지가 전송되는 순서는 보장되지 않는다는 점에 유의하세요.
        /// 참고: 지정한 기간에 최대 4개의 다른 축소 키가 허용됩니다.
        /// 즉, FCM 연결 서버는 클라이언트 앱당 4개의 다른 동기화 전송 메시지를 동시에 저장할 수 있습니다.
        /// 이 한도를 초과하면 FCM 연결 서버가 축소 키 중 어떤 키 4개를 유지할지 보장되지 않습니다.
        /// </summary>
        public string collapse_key
        {
            get; set;
        }

        /// <summary>
        /// 메시지의 우선순위를 설정합니다. 유효한 값은 'normal' 및 'high'입니다. iOS에서는 APN 우선순위 5 및 10에 해당합니다.
        /// 기본적으로 알림 메시지는 높은 우선순위로, 데이터 메시지는 보통 우선순위로 전송됩니다.
        /// 보통 우선순위는 클라이언트 앱의 배터리 소비를 최적화하기 때문에 즉시 전송해야 하는 경우가 아니라면 이 우선순위를 사용하는 것이 좋습니다.
        /// 우선순위가 보통인 메시지의 경우 앱이 메시지를 수신할 때 지정되지 않은 지연이 발생할 수 있습니다.
        /// 높은 우선순위로 메시지를 보내면 즉시 전송되며 앱이 기기의 절전 모드를 해제하고 서버로 연결되는 네트워크 연결을 열 수 있습니다.
        /// 자세한 내용은 메시지 우선순위 설정을 참조하세요.
        /// </summary>
        public string priority
        {
            get; set;
        }

        /// <summary>
        /// 이 매개변수는 메시지 페이로드의 맞춤 키-값 쌍을 지정합니다.
        /// 
        /// 예를 들어 data:{"score":"3x1"}:인 경우 다음과 같습니다.
        /// iOS에서는 메시지가 APNS를 통해 전송되는 경우 맞춤 데이터 필드를 나타냅니다.
        /// FCM 연결 서버를 통해 전송되는 경우에는 AppDelegate application:didReceiveRemoteNotification:의 키와 값으로 구성된 사전으로 표시됩니다.
        /// 
        /// Android에서는 문자열 값이 3x1인 score라는 추가 인텐트가 생성됩니다.
        /// 키가 예약된 단어('google'이나 'gcm'으로 시작하는 모든 단어 또는 'from')여서는 안 됩니다.
        /// 이 표에 정의되어 있는 모든 단어(예: collapse_key)도 사용해서는 안 됩니다.
        /// 문자열 유형의 값을 사용하는 것이 좋습니다.개체의 값이나 문자열이 아닌 기타 데이터 유형(예: 정수 또는 부울)은 문자열로 변환해야 합니다.
        /// </summary>
        public IosNotifyData data
        {
            get; set;
        }

        /// <summary>
        /// 이 매개변수는 사용자에게 표시되는 사전 정의된 알림 페이로드의 키-값 쌍을 지정합니다. 
        /// 자세한 내용은 알림 페이로드 지원을 참조하세요. 
        /// 알림 메시지 및 데이터 메시지 옵션에 대한 자세한 내용은 페이로드를 참조하세요.
        /// </summary>
        public IosNotification notification
        {
            get; set;
        }
    }

    /// <summary>
    /// Android — 알림 메시지 키
    /// </summary>
    public class IosNotification
    {
        /// <summary>
        /// 알림 제목을 나타냅니다. 이 필드는 iOS 휴대전화(iPhone)와 태블릿(iPad)에는 표시되지 않습니다.
        /// </summary>
        public string title
        {
            get; set;
        }

        /// <summary>
        /// 알림 본문 텍스트를 나타냅니다.
        /// </summary>
        public string body
        {
            get; set;
        }

        /// <summary>
        /// 기기가 알림을 수신하면 재생할 사운드를 나타냅니다. 
        /// 사운드 파일은 클라이언트 앱의 메인 번들 또는 앱 데이터 컨테이너의 Library/Sounds 폴더에 위치할 수 있습니다. 
        /// 자세한 내용은 iOS 개발자 라이브러리를 참조하세요.
        /// </summary>
        public string sound
        {
            get; set;
        }

        /// <summary>
        /// 클라이언트 앱 홈 아이콘의 배지를 나타냅니다.
        /// </summary>
        public string badge
        {
            get; set;
        }

        /// <summary>
        /// 사용자의 알림 클릭과 관련된 작업을 나타냅니다. APN 페이로드의 category에 해당합니다.
        /// </summary>
        public string click_action
        {
            get; set;
        }

        /// <summary>
        /// 현지화를 위한 본문 문자열의 키를 나타냅니다. APN 페이로드의 'loc-key'에 해당합니다.
        /// </summary>
        public string body_loc_key
        {
            get; set;
        }

        /// <summary>
        /// 현지화를 위한 본문 문자열의 형식 지정자를 대체하는 문자열 값을 나타냅니다. APN 페이로드의 'loc-args'에 해당합니다.
        /// </summary>
        public string body_loc_args
        {
            get; set;
        }

        /// <summary>
        /// 현지화를 위한 제목 문자열의 키를 나타냅니다.APN 페이로드의 'title-loc-key'에 해당합니다.
        /// </summary>
        public string title_loc_key
        {
            get; set;
        }

        /// <summary>
        /// 현지화를 위한 제목 문자열의 형식 지정자를 대체하는 문자열 값을 나타냅니다.
        /// APN 페이로드의 'title-loc-args'에 해당합니다.
        /// </summary>
        public string title_loc_args
        {
            get; set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IosNotifyData
    {
        /// <summary>
        /// 클라이언트 앱 홈 아이콘의 배지를 나타냅니다.
        /// </summary>
        public int badge
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string title
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string message
        {
            get; set;
        }
    }
}