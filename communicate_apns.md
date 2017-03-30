# [Communicating with APNs](https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CommunicatingwithAPNs.html)

APN(Apple Push Notification) 공급자 API를 사용하면 APN에 원격 알림 요청을 보낼 수 있습니다. 
그런 다음 APN은 iOS, tvOS 및 macOS 기기의 앱과 iOS를 통한 Apple Watch에 알림을 전달합니다.

공급자 API는 HTTP/2 네트워크 프로토콜을 기반으로합니다. 
각 상호 작용은 공급자로부터 JSON 페이로드와 장치 토큰을 포함하는 POST 요청으로 시작됩니다. 
APN은 요청에 포함 된 기기 토큰으로 식별되는 특정 사용자 기기의 앱으로 알림 페이로드를 전달합니다.

공급자는 APN을 사용하도록 구성하고 배포하고 관리하는 서버입니다.


## Provider Authentication Tokens

APN에 안전하게 연결하려면 공급자 인증 토큰 또는 공급자 인증서를 사용할 수 있습니다. 
이 절에서는 토큰을 사용하는 연결에 대해 설명합니다.

공급자 API는 JWT (JSON Web Token) 사양을 지원하므로 각 푸시 알림과 함께 클레임이라는 문과 메타 데이터를 APN에 전달할 수 있습니다. 
자세한 내용은 [https://tools.ietf.org/html/rfc7519](https://tools.ietf.org/html/rfc7519)의 사양을 참조하십시오. 
JWT에 대한 추가 정보와 서명 된 JWT를 생성하는 데 사용할 수있는 라이브러리 목록은 [https://jwt.io](https://jwt.io/)를 참조하십시오.


공급자 인증 토큰은 구성하는 JSON 객체이며 헤더에는 다음을 포함해야합니다.

 - 토큰을 암호화하는 데 사용하는 암호화 알고리즘 (alg)
 - [개발자 계정](https://developer.apple.com/account/)에서 가져온 10 자의 키 식별자 (키) 키


토큰의 클레임 페이로드에는 다음이 포함되어야합니다.

 - 발급자 (iss) 등록 된 클레임 키, [개발자 계정](https://developer.apple.com/account/)에서 가져온 값은 10 자 팀 ID입니다.
 - 발행 된 (iat) 등록 된 클레임 키입니다.이 값은 토큰이 생성 된 시간을 나타내는 값으로, 에포크 이후 경과 한 초 수를 기준으로 UTC 값 입니다.


토큰을 만든 후에는 개인 키로 서명해야합니다. 
그런 다음 P-256 곡선과 SHA-256 해시 알고리즘이있는 ECDSA (Elliptic Curve Digital Signature Algorithm)를 사용하여 토큰을 암호화해야합니다. 
알고리즘 헤더 키 (alg)에 ES256 값을 지정하십시오. 

토큰을 구성하는 방법에 대한 자세한 내용은 Xcode 도움말의 ["푸시 알림 구성"](http://help.apple.com/xcode/mac/current/#/dev11b059073)을 참조하십시오.

APN에 대한 디코드 된 JWT 공급자 인증 토큰의 형식은 다음과 같습니다.:


```json
{
    "alg": "ES256",
    "kid": "ABC123DEFG"
}
{
    "iss": "DEF123GHIJ",
    "iat": 1437179036
 }
 ```

```
(NOTE)
APN은 ES256 알고리즘으로 서명 된 공급자 인증 토큰 만 지원합니다. 
보안되지 않은 JWT 또는 다른 알고리즘으로 서명 된 JWT는 거부되고 공급자는 InvalidProviderToken (403) 응답을 받습니다.
```

보안을 보장하기 위해 APN은 주기적으로 새 토큰을 생성해야합니다. 

새 토큰에는 토큰이 생성 된 시간을 나타내는 claim 키에서 업데이트 된 발급 항목이 있습니다. 
토큰 문제에 대한 타임 스탬프가 지난 1 시간 이내에 있지 않으면 APN은 후속 푸시 메시지를 거부하고 ExpiredProviderToken (403) 오류를 반환합니다.

제공자 토큰 서명 키가 유출 된 것으로 의심되는 경우 [개발자 계정](https://developer.apple.com/account/)에서 해지 할 수 있습니다. 
새 키 쌍을 발행 할 수 있으며 새 개인 키를 사용하여 새 토큰을 생성 할 수 있습니다. 
보안을 최대화하려면 지금 취소 된 키로 서명 된 토큰을 사용하고 있었던 APN에 대한 모든 연결을 닫고 새 키로 서명 된 토큰을 사용하기 전에 다시 연결하십시오.


## APNs Provider Certificates

Xcode 도움말의 "푸시 알림 구성"에서 설명 한 방법으로 APN 제공자 인증서를 사용하면 APN 제작 및 개발 환경에 모두 연결할 수 있습니다.

APN 인증서를 사용하여 번들 ID로 식별되는 기본 앱과 해당 앱과 관련된 모든 Apple Watch 합병증 또는 배경 VoIP 서비스에 알림을 보낼 수 있습니다. 
인증서에서 (1.2.840.113635.100.6.3.6) 확장명을 사용하여 푸시 알림의 주제를 식별하십시오. 
예를 들어, 번들 ID가 com.yourcompany.yourexampleapp 인 앱을 제공하는 경우 인증서에 다음 항목을 지정할 수 있습니다.

```
1. Extension ( 1.2.840.113635.100.6.3.6 )
2. Critical NO
3. Data com.yourcompany.yourexampleapp
4. Data app
5. Data com.yourcompany.yourexampleapp.voip
6. Data voip
7. Data com.yourcompany.yourexampleapp.complication
8. Data complication
```


## APNs Connections

원격 통지를 보내는 첫 번째 단계는 적절한 APN 서버와의 연결을 설정하는 것입니다:

 - Development server: api.development.push.apple.com:443
 - Production server: api.push.apple.com:443

 ```
 NOTE
 APN과 통신 할 때 포트 2197을 사용할 수도 있습니다.
예를 들어 방화벽을 통해 APN 트래픽을 허용하지만 다른 HTTPS 트래픽을 차단할 수 있습니다.
 ```

APN에 연결할 때 공급자가 TLS 1.2 이상을 지원해야합니다. 
[범용 푸시 알림 클라이언트 SSL 인증서 만들기](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html#//apple_ref/doc/uid/TP40012582-CH26-SW11)에 설명 된대로
[개발자 계정](https://developer.apple.com/account/)에서 가져온 공급자 클라이언트 인증서를 사용할 수 있습니다.

APN 공급자 인증서없이 연결하려면 대신 개발자 계정을 통해 제공되는 키로 서명 된 공급자 인증 토큰을 만들어야합니다 (Xcode 도움말의 ["푸시 알림 구성"](http://help.apple.com/xcode/mac/current/#/dev11b059073)참조). 
이 토큰을 갖고 나면 푸시 메시지를 보낼 수 있습니다. 그런 다음 주기적으로 토큰을 업데이트해야합니다. 
각 APNs 제공자 인증 토큰의 유효 기간은 1 시간입니다.

APN은 각 연결에 대해 여러 동시 스트림을 허용합니다. 스트림의 정확한 수는 공급자 인증서 또는 인증 토큰의 사용에 따라 다르며 서버 부하에 따라 다릅니다. 
특정 수의 스트림을 가정하지 마십시오.

인증서가 아닌 토큰을 사용하여 APN에 대한 연결을 설정할 때 유효한 공급자 인증 토큰이있는 푸시 메시지를 보낼 때까지 하나의 스트림 만 연결에 허용됩니다. 
APN은 HTTP/2 PRIORITY 프레임을 무시하므로 스트림에서 보내지 마십시오.

## Best Practices for Managing Connections

여러 알림에 걸쳐 APN 연결을 유지하십시오. 연결을 반복해서 열거 나 닫지 마십시오.
APN은 빠른 연결 및 연결 해제를 서비스 거부 공격으로 간주합니다. 
오랜 시간 동안 유휴 상태가 될 것이라는 것을 모르는 경우를 제외하고는 연결을 열어 두어야합니다. 
예를 들어 하루에 한 번만 사용자에게 알림을 보내면 매일 새 연결을 사용하는 것이 좋습니다.

보내는 푸시 요청마다 새 공급자 인증 토큰을 생성하지 마십시오. 
토큰을 얻은 후에 토큰의 유효 기간 (1 시간) 동안 모든 푸시 요청에 대해 토큰을 계속 사용하십시오.

성능을 향상시키기 위해 APN 서버에 여러 연결을 설정할 수 있습니다. 
많은 수의 원격 통보를 보낼 때 여러 서버 끝점에 대한 연결을 통해 원격 통보를 배포하십시오.
이렇게하면 단일 연결을 사용하는 것과 비교하여 원격 알림을 더 빨리 보내고 APN이 더 빨리 알림을 보내도록함으로써 성능이 향상됩니다.

공급자 인증서가 해지되거나 공급자 토큰에 서명하는 데 사용하는 키가 취소 된 경우 APN에 대한 기존 연결을 모두 닫은 다음 새 연결을 엽니다.

HTTP/2 PING 프레임을 사용하여 연결 상태를 확인할 수 있습니다.


## Terminating an APNs Connection

APN이 설정된 HTTP/2 연결을 종료하기로 결정하면 GOAWAY 프레임을 전송합니다. 
GOAWAY 프레임은 이유 값 키를 사용하여 페이로드에 JSON 데이터를 포함하며 그 값은 연결 종료 이유를 나타냅니다. 
이유 키의 가능한 값 목록은 표 8-6을 참조하십시오.

정상적인 요청 실패로 인해 연결이 종료되지 않습니다.


## APNs Notification API

APNs Provider API는 요청과 HTTP/2 POST 명령을 사용하여 구성하고 보내는 응답으로 구성됩니다. 
요청을 사용하여 APN 서버에 푸시 알림을 보내고 응답을 사용하여 해당 요청의 결과를 결정합니다.


## HTTP/2 Request to APNs

요청을 사용하여 특정 사용자 장치에 알림을 보냅니다.


 <table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-1</strong>HTTP/2 request fields</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Name
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Value
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">:method</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">POST</code>
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">:path</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">/3/device/</code><em>&lt;device-token&gt;</em>
</p></td>
        </tr>
    </tbody>
  </table>


<device-token> 매개 변수의 경우 대상 장치에 대한 장치 토큰의 16 진수 바이트를 지정하십시오.

APN은 HPACK (HTTP/2 용 헤더 압축)을 사용해야 하며, 이는 반복되는 헤더 키와 값을 방지합니다.
APN은 HPACK을위한 작은 동적 테이블을 유지 관리합니다. APNs HPACK 테이블을 채우지 않고 테이블 데이터를 삭제해야하는 경우 다음과 같은 방법으로 헤더를 인코딩하십시오 
- 특히 많은 수의 스트림을 보낼 때 :

 - :path 값은 인덱싱없이 리터럴 헤더 필드로 인코딩되어야합니다.
 - authorization 요청 헤더가있는 경우 인덱싱하지 않고 리터럴 헤더 필드로 인코딩해야합니다.
 - apns-id, apns-expiration 및 apns-collapse-id 요청 헤더에 사용할 적절한 인코딩은 다음과 같이 초기 또는 후속 POST 작업의 일부인지 여부에 따라 다릅니다.

	- 처음으로이 헤더를 보내면 증분 인덱싱으로 인코딩하여 헤더 이름을 동적 테이블에 추가 할 수 있습니다.
	- 이후에이 헤더를 보내고 인덱싱하지 않고 리터럴 헤더 필드로 인코딩합니다.


증분 색인을 사용하여 다른 모든 헤더를 리터럴 헤더 필드로 인코딩합니다. 
헤더 인코딩에 대한 자세한 내용은 [tools.ietf.org/html/rfc7541#section-6.2.1](http://tools.ietf.org/html/rfc7541#section-6.2.1) 
및 [tools.ietf.org/html/rfc7541#section-6.2.2](http://tools.ietf.org/html/rfc7541#section-6.2.2)를 참조하십시오.


 <table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-2</strong>APNs request headers</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Header
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Description
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">authorization</code>
</p></td>
            <td><p class="para">
  The provider token that authorizes APNs to send push notifications for the specified topics. The token is in Base64URL-encoded JWT format, specified as <code class="code-voice">bearer &lt;provider token&gt;</code>. 
</p><p class="para">
  When the provider certificate is used to establish a connection, this request header is ignored.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-id</code>
</p></td>
            <td><p class="para">
  A canonical UUID that identifies the notification. If there is an error sending the notification, APNs uses this value to identify the notification to your server. 
</p><p class="para">
  The canonical form is 32 lowercase hexadecimal digits, displayed in five groups separated by hyphens in the form 8-4-4-4-12. An example UUID is as follows:
</p><p class="para">
  <code class="code-voice">123e4567-e89b-12d3-a456-42665544000</code>
</p><p class="para">
  If you omit this header, a new UUID is created by APNs and returned in the response.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-expiration</code>
</p></td>
            <td><p class="para">
  A UNIX epoch date expressed in seconds (UTC). This header identifies the date when the notification is no longer valid and can be discarded.
</p><p class="para">
  If this value is nonzero, APNs stores the notification and tries to deliver it at least once, repeating the attempt as needed if it is unable to deliver the notification the first time. If the value is <code class="code-voice">0</code>, APNs treats the notification as if it expires immediately and does not store the notification or attempt to redeliver it.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-priority</code>
</p></td>
            <td><p class="para">
  The priority of the notification. Specify one of the following values:
</p><ul class="list-bullet">
  <li class="item"><p class="para">
  <code class="code-voice">10</code>–Send the push message immediately. Notifications with this priority must trigger an alert, sound, or badge on the target device. It is an error to use this priority for a push notification that contains only the <code class="code-voice">content-available</code> key.
</p>
</li><li class="item"><p class="para">
  <code class="code-voice">5</code>—Send the push message at a time that takes into account power considerations for the device. Notifications with this priority might be grouped and delivered in bursts. They are throttled, and in some cases are not delivered.
</p>
</li>
</ul><p class="para">
  If you omit this header, the APNs server sets the priority to <code class="code-voice">10</code>. 
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-topic</code>
</p></td>
            <td><p class="para">
  The topic of the remote notification, which is typically the bundle ID for your app. The certificate you create in your developer account must include the capability for this topic.
</p><p class="para">
  If your certificate includes multiple topics, you must specify a value for this header.
</p><p class="para">
  If you omit this request header and your APNs certificate does not specify multiple topics, the APNs server uses the certificate’s Subject as the default topic.
</p><p class="para">
  If you are using a provider token instead of a certificate, you must specify a value for this request header. The topic you provide should be provisioned for the your team named in your developer account.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-collapse-id</code>
</p></td>
            <td><p class="para">
  Multiple notifications with the same collapse identifier are displayed to the user as a single notification. The value of this key must not exceed 64 bytes. For more information, see <span class="x-name"><a href="APNSOverview.html#//apple_ref/doc/uid/TP40008194-CH8-SW5" data-renderer-version="2" data-id="//apple_ref/doc/uid/TP40008194-CH8-SW5">Quality of Service, Store-and-Forward, and Coalesced Notifications</a></span>.
</p></td>
        </tr>
    </tbody>
</table>



메시지 본문 내용은 알림 페이로드의 JSON 사전 개체입니다. 본문 데이터는 압축하지 않아야하며 최대 크기는 4KB (4096 바이트)입니다. 
VoIP (Voice over Internet Protocol) 알림의 경우 본문 데이터 최대 크기는 5KB (5120 바이트)입니다. 
본문 내용에 포함 할 키와 값에 대한 자세한 내용은 페이로드 키 참조를 참조하십시오.


## HTTP/2 Response from APNs

요청에 대한 응답은 표 8-3에 나열된 형식을 갖습니다.

<table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-3</strong>APNs response headers</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Header name
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Value
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">apns-id</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-id</code> value from the request. If no value was included in the request, the server creates a new UUID and returns it in this header.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">:status</code>
</p></td>
            <td><p class="para">
  The HTTP status code. For a list of possible status codes, see <span class="x-name"><a href="#//apple_ref/doc/uid/TP40008194-CH11-SW15" data-renderer-version="2" data-id="//apple_ref/doc/uid/TP40008194-CH11-SW15">Table 8-4</a></span>.
</p></td>
        </tr>
    </tbody>
  </table>


표 8-4에는 요청의 가능한 상태 코드가 나열되어 있습니다. 이 값은 응답의 :status 헤더에 포함됩니다.



  <table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-4</strong>Status codes for an APNs response</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Status code
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Description
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  200
</p></td>
            <td><p class="para">
  Success
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  400
</p></td>
            <td><p class="para">
  Bad request
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  403
</p></td>
            <td><p class="para">
  There was an error with the certificate or with the provider authentication token
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  405
</p></td>
            <td><p class="para">
  The request used a bad <code class="code-voice">:method</code> value. Only <code class="code-voice">POST</code> requests are supported.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  410
</p></td>
            <td><p class="para">
  The device token is no longer active for the topic.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  413
</p></td>
            <td><p class="para">
  The notification payload was too large.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  429
</p></td>
            <td><p class="para">
  The server received too many requests for the same device token.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  500
</p></td>
            <td><p class="para">
  Internal server error
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  503
</p></td>
            <td><p class="para">
  The server is shutting down and unavailable.
</p></td>
        </tr>
    </tbody>
  </table>




요청이 성공하면 응답 본문이 비어 있습니다. 실패시 응답 본문에는 표 8-5에 나열된 키가있는 JSON 사전이 포함되어 있습니다.
이 JSON 데이터는 연결이 종료되면 GOAWAY 프레임에도 포함될 수 있습니다.


 <table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-5</strong>APNs JSON data keys</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Key
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Description
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">reason</code>
</p></td>
            <td><p class="para">
  The error indicating the reason for the failure. The error code is specified as a string. For a list of possible values, see <span class="x-name"><a href="#//apple_ref/doc/uid/TP40008194-CH11-SW17" data-renderer-version="2" data-id="//apple_ref/doc/uid/TP40008194-CH11-SW17">Table 8-6</a></span>.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">timestamp</code>
</p></td>
            <td><p class="para">
  If the value in the <code class="code-voice">:status</code> header is <code class="code-voice">410</code>, the value of this key is the last time at which APNs confirmed that the device token was no longer valid for the topic.
</p><p class="para">
  Stop pushing notifications until the device registers a token with a later timestamp with your provider.
</p></td>
        </tr>
    </tbody>
  </table>




표 8-6은 응답의 JSON 페이로드의 이유 키에 포함 된 가능한 오류 코드를 나열합니다.



<table class="graybox" border="0" cellspacing="0" cellpadding="5">
    <caption class="tablecaption"><strong class="caption-number">Table 8-6</strong>Values for the APNs JSON <code class="code-voice">reason</code> key</caption>
    <thead>
        <tr>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Status code
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Error string
</p></th>
            <th scope="col" class="TableHeading_TableRow_TableCell"><p class="para">
  Description
</p></th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadCollapseId</code>
</p></td>
            <td><p class="para">
  The collapse identifier exceeds the maximum allowed size
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadDeviceToken</code>
</p></td>
            <td><p class="para">
  The specified device token was bad. Verify that the request contains a valid token and that the token matches the environment.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadExpirationDate</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-expiration</code> value is bad.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadMessageId</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-id</code> value is bad.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadPriority</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-priority</code> value is bad.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadTopic</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-topic</code> was invalid.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">DeviceTokenNotForTopic</code>
</p></td>
            <td><p class="para">
  The device token does not match the specified topic.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">DuplicateHeaders</code>
</p></td>
            <td><p class="para">
  One or more headers were repeated.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">IdleTimeout</code>
</p></td>
            <td><p class="para">
  Idle time out.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">MissingDeviceToken</code>
</p></td>
            <td><p class="para">
  The device token is not specified in the request <code class="code-voice">:path</code>. Verify that the <code class="code-voice">:path</code> header contains the device token.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">MissingTopic</code>
</p></td>
            <td><p class="para">
  The <code class="code-voice">apns-topic</code> header of the request was not specified and was required. The apns-topic header is mandatory when the client is connected using a certificate that supports multiple topics.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">PayloadEmpty</code>
</p></td>
            <td><p class="para">
  The message payload was empty.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">400</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">TopicDisallowed</code>
</p></td>
            <td><p class="para">
  Pushing to this topic is not allowed.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadCertificate</code>
</p></td>
            <td><p class="para">
  The certificate was bad.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadCertificateEnvironment</code>
</p></td>
            <td><p class="para">
  The client certificate was for the wrong environment.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">ExpiredProviderToken</code>
</p></td>
            <td><p class="para">
  The provider token is stale and a new token should be generated.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">Forbidden</code>
</p></td>
            <td><p class="para">
  The specified action is not allowed.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">InvalidProviderToken</code>
</p></td>
            <td><p class="para">
  The provider token is not valid or the token signature could not be verified.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">403</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">MissingProviderToken</code>
</p></td>
            <td><p class="para">
  No provider certificate was used to connect to APNs and Authorization header was missing or no provider token was specified.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">404</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">BadPath</code>
</p></td>
            <td><p class="para">
  The request contained a bad <code class="code-voice">:path</code> value.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">405</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">MethodNotAllowed</code>
</p></td>
            <td><p class="para">
  The specified <code class="code-voice">:method</code> was not <code class="code-voice">POST</code>.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">410</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">Unregistered</code>
</p></td>
            <td><p class="para">
  The device token is inactive for the specified topic.
</p><p class="para">
  Expected HTTP/2 status code is <code class="code-voice">410</code>; see <span class="x-name"><a href="#//apple_ref/doc/uid/TP40008194-CH11-SW15" data-renderer-version="2" data-id="//apple_ref/doc/uid/TP40008194-CH11-SW15">Table 8-4</a></span>. 
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">413</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">PayloadTooLarge</code>
</p></td>
            <td><p class="para">
  The message payload was too large. See <span class="x-name"><a href="CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW1" data-renderer-version="2" data-id="//apple_ref/doc/uid/TP40008194-CH10-SW1">Creating the Remote Notification Payload</a></span> for details on maximum payload size. 
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">429</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">TooManyProviderTokenUpdates</code>
</p></td>
            <td><p class="para">
  The provider token is being updated too often.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">429</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">TooManyRequests</code>
</p></td>
            <td><p class="para">
  Too many requests were made consecutively to the same device token.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">500</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">InternalServerError</code>
</p></td>
            <td><p class="para">
  An internal server error occurred.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">503</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">ServiceUnavailable</code>
</p></td>
            <td><p class="para">
  The service is unavailable.
</p></td>
        </tr>
        <tr>
            <td scope="row"><p class="para">
  <code class="code-voice">503</code>
</p></td>
            <td><p class="para">
  <code class="code-voice">Shutdown</code>
</p></td>
            <td><p class="para">
  The server is shutting down.
</p></td>
        </tr>
    </tbody>
  </table>





## HTTP/2 Request/Response Examples for APNs


Listing 8-1 shows a sample request constructed for a provider certificate.

Listing 8-1Sample request for a certificate with a single topic

```
HEADERS
  - END_STREAM
  + END_HEADERS
  :method = POST
  :scheme = https
  :path = /3/device/00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0
  host = api.development.push.apple.com
  apns-id = eabeae54-14a8-11e5-b60b-1697f925ec7b
  apns-expiration = 0
  apns-priority = 10
DATA
  + END_STREAM
    { "aps" : { "alert" : "Hello" } }
```

Listing 8-2 shows a sample request constructed for a provider authentication token.

Listing 8-2Sample request for a provider authentication token

```
HEADERS
  - END_STREAM
  + END_HEADERS
  :method = POST
  :scheme = https
  :path = /3/device/00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0
  host = api.development.push.apple.com
  authorization = bearer eyAia2lkIjogIjhZTDNHM1JSWDciIH0.eyAiaXNzIjogIkM4Nk5WOUpYM0QiLCAiaWF0I
 jogIjE0NTkxNDM1ODA2NTAiIH0.MEYCIQDzqyahmH1rz1s-LFNkylXEa2lZ_aOCX4daxxTZkVEGzwIhALvkClnx5m5eAT6
 Lxw7LZtEQcH6JENhJTMArwLf3sXwi
  apns-id = eabeae54-14a8-11e5-b60b-1697f925ec7b
  apns-expiration = 0
  apns-priority = 10
  apns-topic = <MyAppTopic>
DATA
  + END_STREAM
    { "aps" : { "alert" : "Hello" } }
```

Listing 8-3 shows a sample request constructed for a certificate that contains multiple topics.

Listing 8-3Sample request for a certificate with multiple topics

```
HEADERS
  - END_STREAM
  + END_HEADERS
  :method = POST
  :scheme = https
  :path = /3/device/00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0
  host = api.development.push.apple.com
  apns-id = eabeae54-14a8-11e5-b60b-1697f925ec7b
  apns-expiration = 0
  apns-priority = 10
  apns-topic = <MyAppTopic> 
DATA
  + END_STREAM
    { "aps" : { "alert" : "Hello" } }
```

Listing 8-4 shows a sample response for a successful push request.

Listing 8-4Sample response for a successful request

```
HEADERS
  + END_STREAM
  + END_HEADERS
  apns-id = eabeae54-14a8-11e5-b60b-1697f925ec7b
  :status = 200
```

Listing 8-5 shows a sample response when an error occurs.

Listing 8-5Sample response for a request that encountered an error

```
HEADERS
  - END_STREAM
  + END_HEADERS
  :status = 400
  content-type = application/json
    apns-id: <a_UUID>
DATA
  + END_STREAM
  { "reason" : "BadDeviceToken" }
```
