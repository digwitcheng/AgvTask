using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCoding.Services
{
    public class ServiceConstantsDef
    {
        public const int MINLEN = 12;
        public const int MAXLEN = 255;
        public const byte COMPENSATION_CMD_HEAD = 0x40;
        public const byte COMPENSATION_CMD_END = 0x2A;

        public const int SEND_MSG_TO_IMS_INTERVIEW_TIME = 10;
        public const int SEND_MSG_TO_IMS_EXCEPTION_INTERVIEW_TIME = 10;

        public const int ANALYZE_IMS_CMD_INTERVIEW_TIME = 10;
        public const int ANALYZE_IMS_CMD_EXCEPTION_INTERVIEW_TIME = 10;

        //public const int CHECK_IMAGE_ARRIVED_INTERVIEW_TIME = 10;
        //public const int CHECK_IMAGE_ARRIVED_MAX_TIMES = 20;

        public const int IMS_RECONN_TIME_SPAN = 5;

        public const int READ_IMAGE_INTERVIEW_TIME = 10;
        public const int READ_IMAGE_EXCEPTION_INTERVIEW_TIME = 10;

        public static volatile byte SeqNum = 0x00;
        public const int PROCESS_IMAGE_SUCESSED = 0;
        public const int PROCESS_IMAGE_FAILED = 0;
        public const int PROCESSING_IMAGE_INTERVIEW_TIME = 5;
        public const int PROCESSING_IMAGE_EXCEPTION_INTERVIEW_TIME = 100;


        #region 消息类型定义
      
        public const Byte GET_COMPENSATION_INFO = 0x6e;
        #endregion

    }
}
