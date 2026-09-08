using System;

namespace ChatBox.Client.Services
{
    /// <summary>
    /// Interface cho dịch vụ gọi video
    /// </summary>
    public interface IVideoCallService
    {
        /// <summary>Bắt đầu gọi video đến 1 user</summary>
        void StartCall(string targetUserId);

        /// <summary>Chấp nhận cuộc gọi</summary>
        void AcceptCall(string callerUserId);

        /// <summary>Từ chối cuộc gọi</summary>
        void RejectCall(string callerUserId);

        /// <summary>Kết thúc cuộc gọi</summary>
        void EndCall();

        /// <summary>Có đang trong cuộc gọi không</summary>
        bool IsInCall { get; }

        /// <summary>User ID đối phương đang đàm thoại</summary>
        string CurrentCallPartner { get; }

        /// <summary>User ID của client hiện tại</summary>
        string CurrentUserId { get; }

        /// <summary>Trạng thái kết nối trực tiếp P2P</summary>
        bool IsP2PConnected { get; }

        /// <summary>Đang dùng Server Relay (khi P2P fail)</summary>
        bool UseRelay { get; }

        /// <summary>Trạng thái bật/tắt camera local</summary>
        bool IsVideoEnabled { get; set; }

        /// <summary>Thay đổi nguồn video (Synthetic / Screen Share / Webcam)</summary>
        void SetVideoSource(IVideoSource source);

        /// <summary>Bật/tắt truyền video</summary>
        void ToggleVideo(bool enabled);

        /// <summary>Event frame nhận từ đối phương</summary>
        event Action<byte[]> OnVideoFrameReceived;

        /// <summary>Event frame camera local (Picture-in-Picture)</summary>
        event Action<byte[]> OnLocalFrameCaptured;

        /// <summary>Event cuộc gọi đến</summary>
        event Action<string> OnIncomingCall;

        /// <summary>Event cuộc gọi được chấp nhận</summary>
        event Action OnCallAccepted;

        /// <summary>Event cuộc gọi bị từ chối</summary>
        event Action<string> OnCallRejected;

        /// <summary>Event cuộc gọi kết thúc</summary>
        event Action OnCallEnded;

        /// <summary>Event log trạng thái</summary>
        event Action<string> OnLog;
    }
}
