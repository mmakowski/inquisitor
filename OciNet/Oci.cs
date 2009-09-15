using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace OciNet
{
    /// <summary>
    /// OCI wrapper for .Net.
    /// 
    /// Conventions for public constants and API wrapper functions:
    /// - OCI prefix is dropped
    /// - Wrapper functions are thin, i.e. resemble the original OCI functions as much as possible;
    /// - IntPtrs are used in place of any single pointers; 
    /// - out arguments are used for pointers to pointers;
    /// - strings are used in place of OraText and other string types. This is done for convenience, to wrap
    ///   the necessary Marshal.StringToHGlobalAnsi() calls;
    /// - overloaded wrappers with rarely used parameters set to default values are provided for convenience
    /// </summary>
    public unsafe class OCI
    {
        #region constants
        // modes
        public const uint DEFAULT = 0x00000000;
        public const uint THREADED = 0x00000001;
        public const uint OBJECT = 0x00000002;
        public const uint EVENTS = 0x00000004;
        public const uint RESERVED1 = 0x00000008;
        public const uint SHARED = 0x00000010;
        public const uint RESERVED2 = 0x00000020;
        public const uint NO_UCB = 0x00000040;
        public const uint NO_MUTEX = 0x00000080;
        public const uint SHARED_EXT = 0x00000100;
        public const uint ALWAYS_BLOCKING = 0x00000400;
        public const uint USE_LDAP = 0x00001000;
        public const uint REG_LDAPONLY = 0x00002000;
        public const uint UTF16 = 0x00004000;
        public const uint AFC_PAD_ON = 0x00008000;
        public const uint ENVCR_RESERVED3 = 0x00010000;
        public const uint NEW_LENGTH_SEMANTICS = 0x00020000;
        public const uint NO_MUTEX_STMT = 0x00040000;
        public const uint MUTEX_ENV_ONLY = 0x00080000;
        public const uint SUPPRESS_NLS_VALIDATION = 0x00100000;
        public const uint MUTEX_TRY = 0x00200000;
        public const uint NCHAR_LITERAL_REPLACE_ON = 0x00400000;
        public const uint NCHAR_LITERAL_REPLACE_OFF = 0x00800000;
        public const uint ENABLE_NLS_VALIDATION = 0x01000000;
        // handle types
        public const uint HTYPE_FIRST = 1;
        public const uint HTYPE_ENV = 1;
        public const uint HTYPE_ERROR = 2;
        public const uint HTYPE_SVCCTX = 3;
        public const uint HTYPE_STMT = 4;
        public const uint HTYPE_BIND = 5;
        public const uint HTYPE_DEFINE = 6;
        public const uint HTYPE_DESCRIBE = 7;
        public const uint HTYPE_SERVER = 8;
        public const uint HTYPE_SESSION = 9;
        public const uint HTYPE_AUTHINFO = HTYPE_SESSION;
        public const uint HTYPE_TRANS = 10;
        public const uint HTYPE_COMPLEXOBJECT = 11;
        public const uint HTYPE_SECURITY = 12;
        public const uint HTYPE_SUBSCRIPTION = 13;
        public const uint HTYPE_DIRPATH_CTX = 14;
        public const uint HTYPE_DIRPATH_COLUMN_ARRAY = 15;
        public const uint HTYPE_DIRPATH_STREAM = 16;
        public const uint HTYPE_PROC = 17;
        public const uint HTYPE_DIRPATH_FN_CTX = 18;
        public const uint HTYPE_DIRPATH_FN_COL_ARRAY = 19;
        public const uint HTYPE_XADSESSION = 20;
        public const uint HTYPE_XADTABLE = 21;
        public const uint HTYPE_XADFIELD = 22;
        public const uint HTYPE_XADGRANULE = 23;
        public const uint HTYPE_XADRECORD = 24;
        public const uint HTYPE_XADIO = 25;
        public const uint HTYPE_CPOOL = 26;
        public const uint HTYPE_SPOOL = 27;
        public const uint HTYPE_ADMIN = 28;
        public const uint HTYPE_EVENT = 29;
        public const uint HTYPE_LAST = 29;
        // errors
        public const int SUCCESS = 0;
        public const int SUCCESS_WITH_INFO = 1;
        public const int RESERVED_FOR_INT_USE = 200;
        public const int NO_DATA = 100;
        public const int ERROR = -1;
        public const int INVALID_HANDLE = -2;
        public const int NEED_DATA = 99;
        public const int STILL_EXECUTING = -3123;
        // attributes
        public const uint ATTR_FNCODE = 1;
        public const uint ATTR_OBJECT = 2;
        public const uint ATTR_NONBLOCKING_MODE = 3;
        public const uint ATTR_SQLCODE = 4;
        public const uint ATTR_ENV = 5;
        public const uint ATTR_SERVER = 6;
        public const uint ATTR_SESSION = 7;
        public const uint ATTR_TRANS = 8;
        public const uint ATTR_ROW_COUNT = 9;
        public const uint ATTR_SQLFNCODE = 10;
        public const uint ATTR_PREFETCH_ROWS = 11;
        public const uint ATTR_NESTED_PREFETCH_ROWS = 12;
        public const uint ATTR_PREFETCH_MEMORY = 13;
        public const uint ATTR_NESTED_PREFETCH_MEMORY = 14;
        public const uint ATTR_CHAR_COUNT = 15;
        public const uint ATTR_PDSCL = 16;
        public const uint ATTR_PDPRC = 17;
        public const uint ATTR_PARAM_COUNT = 18;
        public const uint ATTR_ROWID = 19;
        public const uint ATTR_CHARSET = 20;
        public const uint ATTR_NCHAR = 21;
        public const uint ATTR_USERNAME = 22;
        public const uint ATTR_PASSWORD = 23;
        public const uint ATTR_STMT_TYPE = 24;
        public const uint ATTR_INTERNAL_NAME = 25;
        public const uint ATTR_EXTERNAL_NAME = 26;
        public const uint ATTR_XID = 27;
        public const uint ATTR_TRANS_LOCK = 28;
        public const uint ATTR_TRANS_NAME = 29;
        public const uint ATTR_HEAPALLOC = 30;
        public const uint ATTR_CHARSET_ID = 31;
        public const uint ATTR_CHARSET_FORM = 32;
        public const uint ATTR_MAXDATA_SIZE = 33;
        public const uint ATTR_CACHE_OPT_SIZE = 34;
        public const uint ATTR_CACHE_MAX_SIZE = 35;
        public const uint ATTR_PINOPTION = 36;
        public const uint ATTR_ALLOC_DURATION = 37;
        public const uint ATTR_PIN_DURATION = 38;
        public const uint ATTR_FDO = 39;
        public const uint ATTR_POSTPROCESSING_CALLBACK = 40;
        public const uint ATTR_POSTPROCESSING_CONTEXT = 41;
        public const uint ATTR_ROWS_RETURNED = 42;
        public const uint ATTR_FOCBK = 43;
        public const uint ATTR_IN_V8_MODE = 44;
        public const uint ATTR_LOBEMPTY = 45;
        public const uint ATTR_SESSLANG = 46;
        public const uint ATTR_VISIBILITY = 47;
        public const uint ATTR_RELATIVE_MSGID = 48;
        public const uint ATTR_SEQUENCE_DEVIATION = 49;
        public const uint ATTR_CONSUMER_NAME = 50;
        public const uint ATTR_DEQ_MODE = 51;
        public const uint ATTR_NAVIGATION = 52;
        public const uint ATTR_WAIT = 53;
        public const uint ATTR_DEQ_MSGID = 54;
        public const uint ATTR_PRIORITY = 55;
        public const uint ATTR_DELAY = 56;
        public const uint ATTR_EXPIRATION = 57;
        public const uint ATTR_CORRELATION = 58;
        public const uint ATTR_ATTEMPTS = 59;
        public const uint ATTR_RECIPIENT_LIST = 60;
        public const uint ATTR_EXCEPTION_QUEUE = 61;
        public const uint ATTR_ENQ_TIME = 62;
        public const uint ATTR_MSG_STATE = 63;
        public const uint ATTR_AGENT_NAME = 64;
        public const uint ATTR_AGENT_ADDRESS = 65;
        public const uint ATTR_AGENT_PROTOCOL = 66;
        public const uint ATTR_USER_PROPERTY = 67;
        public const uint ATTR_SENDER_ID = 68;
        public const uint ATTR_ORIGINAL_MSGID = 69;
        public const uint ATTR_QUEUE_NAME = 70;
        public const uint ATTR_NFY_MSGID = 71;
        public const uint ATTR_MSG_PROP = 72;
        public const uint ATTR_NUM_DML_ERRORS = 73;
        public const uint ATTR_DML_ROW_OFFSET = 74;
        public const uint ATTR_AQ_NUM_ERRORS = ATTR_NUM_DML_ERRORS;
        public const uint ATTR_AQ_ERROR_INDEX = ATTR_DML_ROW_OFFSET;
        public const uint ATTR_DATEFORMAT = 75;
        public const uint ATTR_BUF_ADDR = 76;
        public const uint ATTR_BUF_SIZE = 77;
        public const uint ATTR_NUM_ROWS = 81;
        public const uint ATTR_COL_COUNT = 82;
        public const uint ATTR_STREAM_OFFSET = 83;
        public const uint ATTR_SHARED_HEAPALLOC = 84;
        public const uint ATTR_SERVER_GROUP = 85;
        public const uint ATTR_MIGSESSION = 86;
        public const uint ATTR_NOCACHE = 87;
        public const uint ATTR_MEMPOOL_SIZE = 88;
        public const uint ATTR_MEMPOOL_INSTNAME = 89;
        public const uint ATTR_MEMPOOL_APPNAME = 90;
        public const uint ATTR_MEMPOOL_HOMENAME = 91;
        public const uint ATTR_MEMPOOL_MODEL = 92;
        public const uint ATTR_MODES = 93;
        public const uint ATTR_SUBSCR_NAME = 94;
        public const uint ATTR_SUBSCR_CALLBACK = 95;
        public const uint ATTR_SUBSCR_CTX = 96;
        public const uint ATTR_SUBSCR_PAYLOAD = 97;
        public const uint ATTR_SUBSCR_NAMESPACE = 98;
        public const uint ATTR_PROXY_CREDENTIALS = 99;
        public const uint ATTR_INITIAL_CLIENT_ROLES = 100;
        public const uint ATTR_UNK = 101;
        public const uint ATTR_NUM_COLS = 102;
        public const uint ATTR_LIST_COLUMNS = 103;
        public const uint ATTR_RDBA = 104;
        public const uint ATTR_CLUSTERED = 105;
        public const uint ATTR_PARTITIONED = 106;
        public const uint ATTR_INDEX_ONLY = 107;
        public const uint ATTR_LIST_ARGUMENTS = 108;
        public const uint ATTR_LIST_SUBPROGRAMS = 109;
        public const uint ATTR_REF_TDO = 110;
        public const uint ATTR_LINK = 111;
        public const uint ATTR_MIN = 112;
        public const uint ATTR_MAX = 113;
        public const uint ATTR_INCR = 114;
        public const uint ATTR_CACHE = 115;
        public const uint ATTR_ORDER = 116;
        public const uint ATTR_HW_MARK = 117;
        public const uint ATTR_TYPE_SCHEMA = 118;
        public const uint ATTR_TIMESTAMP = 119;
        public const uint ATTR_NUM_ATTRS = 120;
        public const uint ATTR_NUM_PARAMS = 121;
        public const uint ATTR_OBJID = 122;
        public const uint ATTR_PTYPE = 123;
        public const uint ATTR_PARAM = 124;
        public const uint ATTR_OVERLOAD_ID = 125;
        public const uint ATTR_TABLESPACE = 126;
        public const uint ATTR_TDO = 127;
        public const uint ATTR_LTYPE = 128;
        public const uint ATTR_PARSE_ERROR_OFFSET = 129;
        public const uint ATTR_IS_TEMPORARY = 130;
        public const uint ATTR_IS_TYPED = 131;
        public const uint ATTR_DURATION = 132;
        public const uint ATTR_IS_INVOKER_RIGHTS = 133;
        public const uint ATTR_OBJ_NAME = 134;
        public const uint ATTR_OBJ_SCHEMA = 135;
        public const uint ATTR_OBJ_ID = 136;
        public const uint ATTR_TRANS_TIMEOUT = 142;
        public const uint ATTR_SERVER_STATUS = 143;
        public const uint ATTR_STATEMENT = 144;
        public const uint ATTR_DEQCOND = 146;
        public const uint ATTR_RESERVED_2 = 147;
        public const uint ATTR_SUBSCR_RECPT = 148;
        public const uint ATTR_SUBSCR_RECPTPROTO = 149;
        public const uint ATTR_LDAP_HOST = 153;
        public const uint ATTR_LDAP_PORT = 154;
        public const uint ATTR_BIND_DN = 155;
        public const uint ATTR_LDAP_CRED = 156;
        public const uint ATTR_WALL_LOC = 157;
        public const uint ATTR_LDAP_AUTH = 158;
        public const uint ATTR_LDAP_CTX = 159;
        public const uint ATTR_SERVER_DNS = 160;
        public const uint ATTR_DN_COUNT = 161;
        public const uint ATTR_SERVER_DN = 162;
        public const uint ATTR_MAXCHAR_SIZE = 163;
        public const uint ATTR_CURRENT_POSITION = 164;
        public const uint ATTR_RESERVED_3 = 165;
        public const uint ATTR_RESERVED_4 = 166;
        public const uint ATTR_DIGEST_ALGO = 168;
        public const uint ATTR_CERTIFICATE = 169;
        public const uint ATTR_SIGNATURE_ALGO = 170;
        public const uint ATTR_CANONICAL_ALGO = 171;
        public const uint ATTR_PRIVATE_KEY = 172;
        public const uint ATTR_DIGEST_VALUE = 173;
        public const uint ATTR_SIGNATURE_VAL = 174;
        public const uint ATTR_SIGNATURE = 175;
        public const uint ATTR_STMTCACHESIZE = 176;
        public const uint ATTR_CONN_NOWAIT = 178;
        public const uint ATTR_CONN_BUSY_COUNT = 179;
        public const uint ATTR_CONN_OPEN_COUNT = 180;
        public const uint ATTR_CONN_TIMEOUT = 181;
        public const uint ATTR_STMT_STATE = 182;
        public const uint ATTR_CONN_MIN = 183;
        public const uint ATTR_CONN_MAX = 184;
        public const uint ATTR_CONN_INCR = 185;
        public const uint ATTR_NUM_OPEN_STMTS = 188;
        public const uint ATTR_DESCRIBE_NATIVE = 189;
        public const uint ATTR_BIND_COUNT = 190;
        public const uint ATTR_HANDLE_POSITION = 191;
        public const uint ATTR_RESERVED_5 = 192;
        public const uint ATTR_SERVER_BUSY = 193;
        public const uint ATTR_SUBSCR_RECPTPRES = 195;
        public const uint ATTR_TRANSFORMATION = 196;
        public const uint ATTR_ROWS_FETCHED = 197;
        public const uint ATTR_SCN_BASE = 198;
        public const uint ATTR_SCN_WRAP = 199;
        public const uint ATTR_RESERVED_6 = 200;
        public const uint ATTR_READONLY_TXN = 201;
        public const uint ATTR_RESERVED_7 = 202;
        public const uint ATTR_ERRONEOUS_COLUMN = 203;
        public const uint ATTR_RESERVED_8 = 204;
        public const uint ATTR_ASM_VOL_SPRT = 205;
        public const uint ATTR_INST_TYPE = 207;
        public const uint ATTR_ENV_UTF16 = 209;
        public const uint ATTR_RESERVED_9 = 210;
        public const uint ATTR_RESERVED_10 = 211;
        public const uint ATTR_RESERVED_12 = 214;
        public const uint ATTR_RESERVED_13 = 215;
        public const uint ATTR_IS_EXTERNAL = 216;
        public const uint ATTR_RESERVED_15 = 217;
        public const uint ATTR_STMT_IS_RETURNING = 218;
        public const uint ATTR_RESERVED_16 = 219;
        public const uint ATTR_RESERVED_17 = 220;
        public const uint ATTR_RESERVED_18 = 221;
        public const uint ATTR_RESERVED_19 = 222;
        public const uint ATTR_RESERVED_20 = 223;
        public const uint ATTR_CURRENT_SCHEMA = 224;
        public const uint ATTR_RESERVED_21 = 415;
        public const uint ATTR_SUBSCR_QOSFLAGS = 225;
        public const uint ATTR_SUBSCR_PAYLOADCBK = 226;
        public const uint ATTR_SUBSCR_TIMEOUT = 227;
        public const uint ATTR_SUBSCR_NAMESPACE_CTX = 228;
        public const uint ATTR_SUBSCR_CQ_QOSFLAGS = 229;
        public const uint ATTR_SUBSCR_CQ_REGID = 230;
        public const uint ATTR_SUBSCR_NTFN_GROUPING_CLASS = 231;
        public const uint ATTR_SUBSCR_NTFN_GROUPING_VALUE = 232;
        public const uint ATTR_SUBSCR_NTFN_GROUPING_TYPE = 233;
        public const uint ATTR_SUBSCR_NTFN_GROUPING_START_TIME = 234;
        public const uint ATTR_SUBSCR_NTFN_GROUPING_REPEAT_COUNT = 235;
        public const uint ATTR_AQ_NTFN_GROUPING_MSGID_ARRAY = 236;
        public const uint ATTR_AQ_NTFN_GROUPING_COUNT = 237;
        public const uint ATTR_BIND_ROWCBK = 301;
        public const uint ATTR_BIND_ROWCTX = 302;
        public const uint ATTR_SKIP_BUFFER = 303;
        public const uint ATTR_CQ_QUERYID = 304;
        public const uint ATTR_CHNF_TABLENAMES = 401;
        public const uint ATTR_CHNF_ROWIDS = 402;
        public const uint ATTR_CHNF_OPERATIONS = 403;
        public const uint ATTR_CHNF_CHANGELAG = 404;
        public const uint ATTR_CHDES_DBNAME = 405;
        public const uint ATTR_CHDES_NFYTYPE = 406;
        public const uint ATTR_CHDES_XID = 407;
        public const uint ATTR_CHDES_TABLE_CHANGES = 408;
        public const uint ATTR_CHDES_TABLE_NAME = 409;
        public const uint ATTR_CHDES_TABLE_OPFLAGS = 410;
        public const uint ATTR_CHDES_TABLE_ROW_CHANGES = 411;
        public const uint ATTR_CHDES_ROW_ROWID = 412;
        public const uint ATTR_CHDES_ROW_OPFLAGS = 413;
        public const uint ATTR_CHNF_REGHANDLE = 414;
        public const uint ATTR_NETWORK_FILE_DESC = 415;
        public const uint ATTR_PROXY_CLIENT = 416;
        public const uint ATTR_TABLE_ENC = 417;
        public const uint ATTR_TABLE_ENC_ALG = 418;
        public const uint ATTR_TABLE_ENC_ALG_ID = 419;
        public const uint ATTR_STMTCACHE_CBKCTX = 420;
        public const uint ATTR_STMTCACHE_CBK = 421;
        public const uint ATTR_CQDES_OPERATION = 422;
        public const uint ATTR_CQDES_TABLE_CHANGES = 423;
        public const uint ATTR_CQDES_QUERYID = 424;
        public const uint ATTR_CHDES_QUERIES = 425;
        public const uint ATTR_RESERVED_26 = 422;
        public const uint ATTR_CONNECTION_CLASS = 425;
        public const uint ATTR_PURITY = 426;
        public const uint ATTR_PURITY_DEFAULT = 0x00;
        public const uint ATTR_PURITY_NEW = 0x01;
        public const uint ATTR_PURITY_SELF = 0x02;
        public const uint ATTR_RESERVED_28 = 426;
        public const uint ATTR_RESERVED_29 = 427;
        public const uint ATTR_RESERVED_30 = 428;
        public const uint ATTR_RESERVED_31 = 429;
        public const uint ATTR_RESERVED_32 = 430;
        public const uint ATTR_RESERVED_41 = 454;
        public const uint ATTR_RESERVED_33 = 433;
        public const uint ATTR_RESERVED_34 = 434;
        public const uint ATTR_RESERVED_36 = 444;
        public const uint ATTR_SEND_TIMEOUT = 435;
        public const uint ATTR_RECEIVE_TIMEOUT = 436;
        public const uint ATTR_DEFAULT_LOBPREFETCH_SIZE = 438;
        public const uint ATTR_LOBPREFETCH_SIZE = 439;
        public const uint ATTR_LOBPREFETCH_LENGTH = 440;
        public const uint ATTR_LOB_REGION_PRIMARY = 442;
        public const uint ATTR_LOB_REGION_PRIMOFF = 443;
        public const uint ATTR_LOB_REGION_OFFSET = 445;
        public const uint ATTR_LOB_REGION_LENGTH = 446;
        public const uint ATTR_LOB_REGION_MIME = 447;
        public const uint ATTR_FETCH_ROWID = 448;
        public const uint ATTR_RESERVED_37 = 449;
        // handle parameter attributes
        public const uint ATTR_DATA_SIZE = 1;
        public const uint ATTR_DATA_TYPE = 2;
        public const uint ATTR_DISP_SIZE = 3;
        public const uint ATTR_NAME = 4;
        public const uint ATTR_PRECISION = 5;
        public const uint ATTR_SCALE = 6;
        public const uint ATTR_IS_NULL = 7;
        public const uint ATTR_TYPE_NAME = 8;
        public const uint ATTR_SCHEMA_NAME = 9;
        public const uint ATTR_SUB_NAME = 10;
        public const uint ATTR_POSITION = 11;
        public const uint ATTR_COMPLEXOBJECTCOMP_TYPE = 50;
        public const uint ATTR_COMPLEXOBJECTCOMP_TYPE_LEVEL = 51;
        public const uint ATTR_COMPLEXOBJECT_LEVEL = 52;
        public const uint ATTR_COMPLEXOBJECT_COLL_OUTOFLINE = 53;
        public const uint ATTR_DISP_NAME = 100;
        public const uint ATTR_ENCC_SIZE = 101;
        public const uint ATTR_COL_ENC = 102;
        public const uint ATTR_COL_ENC_SALT = 103;
        public const uint ATTR_OVERLOAD = 210;
        public const uint ATTR_LEVEL = 211;
        public const uint ATTR_HAS_DEFAULT = 212;
        public const uint ATTR_IOMODE = 213;
        public const uint ATTR_RADIX = 214;
        public const uint ATTR_NUM_ARGS = 215;
        public const uint ATTR_TYPECODE = 216;
        public const uint ATTR_COLLECTION_TYPECODE = 217;
        public const uint ATTR_VERSION = 218;
        public const uint ATTR_IS_INCOMPLETE_TYPE = 219;
        public const uint ATTR_IS_SYSTEM_TYPE = 220;
        public const uint ATTR_IS_PREDEFINED_TYPE = 221;
        public const uint ATTR_IS_TRANSIENT_TYPE = 222;
        public const uint ATTR_IS_SYSTEM_GENERATED_TYPE = 223;
        public const uint ATTR_HAS_NESTED_TABLE = 224;
        public const uint ATTR_HAS_LOB = 225;
        public const uint ATTR_HAS_FILE = 226;
        public const uint ATTR_COLLECTION_ELEMENT = 227;
        public const uint ATTR_NUM_TYPE_ATTRS = 228;
        public const uint ATTR_LIST_TYPE_ATTRS = 229;
        public const uint ATTR_NUM_TYPE_METHODS = 230;
        public const uint ATTR_LIST_TYPE_METHODS = 231;
        public const uint ATTR_MAP_METHOD = 232;
        public const uint ATTR_ORDER_METHOD = 233;
        public const uint ATTR_NUM_ELEMS = 234;
        public const uint ATTR_ENCAPSULATION = 235;
        public const uint ATTR_IS_SELFISH = 236;
        public const uint ATTR_IS_VIRTUAL = 237;
        public const uint ATTR_IS_INLINE = 238;
        public const uint ATTR_IS_CONSTANT = 239;
        public const uint ATTR_HAS_RESULT = 240;
        public const uint ATTR_IS_CONSTRUCTOR = 241;
        public const uint ATTR_IS_DESTRUCTOR = 242;
        public const uint ATTR_IS_OPERATOR = 243;
        public const uint ATTR_IS_MAP = 244;
        public const uint ATTR_IS_ORDER = 245;
        public const uint ATTR_IS_RNDS = 246;
        public const uint ATTR_IS_RNPS = 247;
        public const uint ATTR_IS_WNDS = 248;
        public const uint ATTR_IS_WNPS = 249;
        public const uint ATTR_DESC_PUBLIC = 250;
        public const uint ATTR_CACHE_CLIENT_CONTEXT = 251;
        public const uint ATTR_UCI_CONSTRUCT = 252;
        public const uint ATTR_UCI_DESTRUCT = 253;
        public const uint ATTR_UCI_COPY = 254;
        public const uint ATTR_UCI_PICKLE = 255;
        public const uint ATTR_UCI_UNPICKLE = 256;
        public const uint ATTR_UCI_REFRESH = 257;
        public const uint ATTR_IS_SUBTYPE = 258;
        public const uint ATTR_SUPERTYPE_SCHEMA_NAME = 259;
        public const uint ATTR_SUPERTYPE_NAME = 260;
        public const uint ATTR_LIST_OBJECTS = 261;
        public const uint ATTR_NCHARSET_ID = 262;
        public const uint ATTR_LIST_SCHEMAS = 263;
        public const uint ATTR_MAX_PROC_LEN = 264;
        public const uint ATTR_MAX_COLUMN_LEN = 265;
        public const uint ATTR_CURSOR_COMMIT_BEHAVIOR = 266;
        public const uint ATTR_MAX_CATALOG_NAMELEN = 267;
        public const uint ATTR_CATALOG_LOCATION = 268;
        public const uint ATTR_SAVEPOINT_SUPPORT = 269;
        public const uint ATTR_NOWAIT_SUPPORT = 270;
        public const uint ATTR_AUTOCOMMIT_DDL = 271;
        public const uint ATTR_LOCKING_MODE = 272;
        public const uint ATTR_APPCTX_SIZE = 273;
        public const uint ATTR_APPCTX_LIST = 274;
        public const uint ATTR_APPCTX_NAME = 275;
        public const uint ATTR_APPCTX_ATTR = 276;
        public const uint ATTR_APPCTX_VALUE = 277;
        public const uint ATTR_CLIENT_IDENTIFIER = 278;
        public const uint ATTR_IS_FINAL_TYPE = 279;
        public const uint ATTR_IS_INSTANTIABLE_TYPE = 280;
        public const uint ATTR_IS_FINAL_METHOD = 281;
        public const uint ATTR_IS_INSTANTIABLE_METHOD = 282;
        public const uint ATTR_IS_OVERRIDING_METHOD = 283;
        public const uint ATTR_DESC_SYNBASE = 284;
        public const uint ATTR_CHAR_USED = 285;
        public const uint ATTR_CHAR_SIZE = 286;
        public const uint ATTR_IS_JAVA_TYPE = 287;
        public const uint ATTR_DISTINGUISHED_NAME = 300;
        public const uint ATTR_KERBEROS_TICKET = 301;
        public const uint ATTR_ORA_DEBUG_JDWP = 302;
        public const uint ATTR_EDITION = 288;
        public const uint ATTR_RESERVED_14 = 303;
        // credential types
        public const uint CRED_RDBMS = 1;
        public const uint CRED_EXT = 2;
        public const uint CRED_PROXY = 3;
        public const uint CRED_RESERVED_1 = 4;
        public const uint CRED_RESERVED_2 = 5;
        // syntax types
        public const uint V7_SYNTAX = 2;
        public const uint V8_SYNTAX = 3;
        public const uint NTV_SYNTAX = 1;
        // descriptor types
        public const uint DTYPE_FIRST = 50;
        public const uint DTYPE_LOB = 50;
        public const uint DTYPE_SNAP = 51;
        public const uint DTYPE_RSET = 52;
        public const uint DTYPE_PARAM = 53;
        public const uint DTYPE_ROWID = 54;
        public const uint DTYPE_COMPLEXOBJECTCOMP = 55;
        public const uint DTYPE_FILE = 56;
        public const uint DTYPE_AQENQ_OPTIONS = 57;
        public const uint DTYPE_AQDEQ_OPTIONS = 58;
        public const uint DTYPE_AQMSG_PROPERTIES = 59;
        public const uint DTYPE_AQAGENT = 60;
        public const uint DTYPE_LOCATOR = 61;
        public const uint DTYPE_INTERVAL_YM = 62;
        public const uint DTYPE_INTERVAL_DS = 63;
        public const uint DTYPE_AQNFY_DESCRIPTOR = 64;
        public const uint DTYPE_DATE = 65;
        public const uint DTYPE_TIME = 66;
        public const uint DTYPE_TIME_TZ = 67;
        public const uint DTYPE_TIMESTAMP = 68;
        public const uint DTYPE_TIMESTAMP_TZ = 69;
        public const uint DTYPE_TIMESTAMP_LTZ = 70;
        public const uint DTYPE_UCB = 71;
        public const uint DTYPE_SRVDN = 72;
        public const uint DTYPE_SIGNATURE = 73;
        public const uint DTYPE_RESERVED_1 = 74;
        public const uint DTYPE_AQLIS_OPTIONS = 75;
        public const uint DTYPE_AQLIS_MSG_PROPERTIES = 76;
        public const uint DTYPE_CHDES = 77;
        public const uint DTYPE_TABLE_CHDES = 78;
        public const uint DTYPE_ROW_CHDES = 79;
        public const uint DTYPE_CQDES = 80;
        public const uint DTYPE_LOB_REGION = 81;
        public const uint DTYPE_LAST = 81;
        // fetch types
        public const uint FETCH_CURRENT = 0x00000001;
        public const uint FETCH_NEXT = 0x00000002;
        public const uint FETCH_FIRST = 0x00000004;
        public const uint FETCH_LAST = 0x00000008;
        public const uint FETCH_PRIOR = 0x00000010;
        public const uint FETCH_ABSOLUTE = 0x00000020;
        public const uint FETCH_RELATIVE = 0x00000040;
        public const uint FETCH_RESERVED_1 = 0x00000080;
        public const uint FETCH_RESERVED_2 = 0x00000100;
        public const uint FETCH_RESERVED_3 = 0x00000200;
        public const uint FETCH_RESERVED_4 = 0x00000400;
        public const uint FETCH_RESERVED_5 = 0x00000800;
        #endregion

        #region wrappers for OCI functions

        // -- OCIAttrGet
        [DllImport("oci.dll", EntryPoint = "OCIAttrGet")]
        private static extern int OCIAttrGet(void* trgthndlp, uint trghndltyp, void* attributep /*out*/, void* sizep /*out*/, uint attrtype, void* errhp);
        public static int AttrGet(IntPtr trgthndlp, uint trghndltyp, out IntPtr attributep, out uint sizep, uint attrtype, IntPtr errhp)
        {
            IntPtr attributepPtr = IntPtr.Zero;
            IntPtr sizepPtr = IntPtr.Zero;
            try
            {
                attributepPtr = Marshal.AllocHGlobal(IntPtr.Size);
                sizepPtr = Marshal.AllocHGlobal(sizeof(uint));
                // TODO: what happens with the memory allocated by OCI to hold attribute value? It doesn't seem to get deallocated
                int err = OCIAttrGet(trgthndlp.ToPointer(), trghndltyp, attributepPtr.ToPointer(), sizepPtr.ToPointer(), attrtype, errhp.ToPointer());
                attributep = Marshal.ReadIntPtr(attributepPtr);
                sizep = (uint)Marshal.ReadInt32(sizepPtr);
                return err;
            }
            finally
            {
                if (attributepPtr != IntPtr.Zero) Marshal.FreeHGlobal(attributepPtr);
                if (sizepPtr != IntPtr.Zero) Marshal.FreeHGlobal(sizepPtr);
            }
        }
        public static int AttrGet(IntPtr trgthndlp, uint trghndltyp, out string attributep, out uint sizep, uint attrtype, IntPtr errhp)
        {
            IntPtr attributepPtr = IntPtr.Zero;
            int err = AttrGet(trgthndlp, trghndltyp, out attributepPtr, out sizep, attrtype, errhp);
            attributep = Marshal.PtrToStringAnsi(attributepPtr);
            return err;
        }

        // -- OCIAttrSet
        [DllImport("oci.dll", EntryPoint = "OCIAttrSet")]
        private static extern int OCIAttrSet(void* trgthndlp, uint trghndltyp, void* attributep, uint size, uint attrtype, void* errhp);
        public static int AttrSet(IntPtr trgthndlp, uint trghndltyp, IntPtr attributep, uint size, uint attrtype, IntPtr errhp)
        {
            return OCIAttrSet(trgthndlp.ToPointer(), trghndltyp, attributep.ToPointer(), size, attrtype, errhp.ToPointer());
        }
        public static int AttrSet(IntPtr trgthndlp, uint trghndltyp, string attributep, uint size, uint attrtype, IntPtr errhp)
        {
            IntPtr attributepPtr = IntPtr.Zero;
            try
            {
                attributepPtr = Marshal.StringToHGlobalAnsi(attributep);
                return AttrSet(trgthndlp, trghndltyp, attributepPtr, size, attrtype, errhp);
            }
            finally
            {
                if (attributepPtr != IntPtr.Zero) Marshal.FreeHGlobal(attributepPtr);
            }
        }

        // -- OCIEnvCreate
        [DllImport("oci.dll", EntryPoint = "OCIEnvCreate")]
        private static extern int OCIEnvCreate(void* envpp /*out*/, uint mode, void* ctxp, void* malocfp, void* ralocfp, void* mfreefp, uint xtramem_sz, void* usrmempp /*out*/);
        // NOTE: xtramem_sz and usrmempp arguments are ignored (OCI returns error when non-zero pointer is passed as usrmempp)
        public static int EnvCreate(out IntPtr envpp, uint mode, IntPtr ctxp, IntPtr malocfp, IntPtr ralocfp, IntPtr mfreefp, uint xtramem_sz, out IntPtr usrmempp)
        {
            IntPtr envppPtr = IntPtr.Zero;
            try
            {
                envppPtr = Marshal.AllocHGlobal(IntPtr.Size);
                int err = OCIEnvCreate(envppPtr.ToPointer(), mode, ctxp.ToPointer(), malocfp.ToPointer(), ralocfp.ToPointer(), mfreefp.ToPointer(), 0, IntPtr.Zero.ToPointer());
                envpp = Marshal.ReadIntPtr(envppPtr);
                usrmempp = IntPtr.Zero;
                return err;
            }
            finally
            {
                if (envppPtr != IntPtr.Zero) Marshal.FreeHGlobal(envppPtr);
            }
        }
        public static int EnvCreate(out IntPtr envpp, uint mode)
        {
            IntPtr dummy = IntPtr.Zero;
            return EnvCreate(out envpp, mode, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, out dummy);
        }

        // -- OCIErrorGet
        [DllImport("oci.dll", EntryPoint = "OCIErrorGet")]
        private static extern int OCIErrorGet(void* hndlp, uint recordno, void* sqlstate /*out*/, void* errcodep /*out*/, void* bufp /*out*/, uint bufsiz, uint type);
        public static int ErrorGet(IntPtr hndlp, uint recordno, out string sqlstate, out uint errcodep, out string bufp, uint bufsiz, uint type)
        {
            IntPtr errcodepPtr = IntPtr.Zero;
            IntPtr bufpPtr = IntPtr.Zero;
            try
            {
                errcodepPtr = Marshal.AllocHGlobal(sizeof(uint));
                bufpPtr = Marshal.AllocHGlobal((int) bufsiz);
                int err = OCIErrorGet(hndlp.ToPointer(), recordno, IntPtr.Zero.ToPointer(), errcodepPtr.ToPointer(), bufpPtr.ToPointer(), bufsiz, type);
                sqlstate = null;
                errcodep = (uint) Marshal.ReadInt32(errcodepPtr);
                bufp = Marshal.PtrToStringAnsi(bufpPtr);
                return err;
            }
            finally
            {
                if (errcodepPtr != IntPtr.Zero) Marshal.FreeHGlobal(errcodepPtr);
                if (bufpPtr != IntPtr.Zero) Marshal.FreeHGlobal(bufpPtr);
            }
        }

        // -- OCIHandleAlloc
        [DllImport("oci.dll", EntryPoint = "OCIHandleAlloc")]
        private static extern int OCIHandleAlloc(void* parenth, void* hndlpp /*out*/, uint type, uint xtramem_sz, void* usrmempp /*out*/);
        // NOTE: xtramem_sz and usrmempp arguments are ignored (OCI returns error when non-zero pointer is passed as usrmempp)
        public static int HandleAlloc(IntPtr parenth, out IntPtr hndlpp, uint type, uint xtramem_sz, out IntPtr usrmempp)
        {
            IntPtr hndlppPtr = IntPtr.Zero;
            try
            {
                hndlppPtr = Marshal.AllocHGlobal(IntPtr.Size);
                int err = OCIHandleAlloc(parenth.ToPointer(), hndlppPtr.ToPointer(), type, 0, IntPtr.Zero.ToPointer());
                hndlpp = Marshal.ReadIntPtr(hndlppPtr);
                usrmempp = IntPtr.Zero;
                return err;
            }
            finally
            {
                if (hndlppPtr != IntPtr.Zero) Marshal.FreeHGlobal(hndlppPtr);
            }
        }
        public static int HandleAlloc(IntPtr parenth, out IntPtr hndlpp, uint type)
        {
            IntPtr dummy = IntPtr.Zero;
            return HandleAlloc(parenth, out hndlpp, type, 0, out dummy);
        }

        // -- OCIHandleFree
        [DllImport("oci.dll", EntryPoint = "OCIHandleFree")]
        private static extern int OCIHandleFree(void* hndlp, uint type);
        public static int HandleFree(IntPtr handle, uint type)
        {
            return OCIHandleFree(handle.ToPointer(), type);
        }

        // -- OCIParamGet
        [DllImport("oci.dll", EntryPoint = "OCIParamGet")]
        private static extern int OCIParamGet(void* hndlp, uint htype, void* errhp, void* paramdpp /*out*/, uint pos);
        public static int ParamGet(IntPtr hndlp, uint htype, IntPtr errhp, out IntPtr paramdpp, uint pos)
        {
            IntPtr paramdppPtr = IntPtr.Zero;
            try
            {
                paramdppPtr = Marshal.AllocHGlobal(IntPtr.Size);
                int err = OCIParamGet(hndlp.ToPointer(), htype, errhp.ToPointer(), paramdppPtr.ToPointer(), pos);
                paramdpp = Marshal.ReadIntPtr(paramdppPtr);
                return err;
            }
            finally
            {
                if (paramdppPtr != IntPtr.Zero) Marshal.FreeHGlobal(paramdppPtr);
            }
        }

        // -- OCIServerAttach
        [DllImport("oci.dll", EntryPoint = "OCIServerAttach")]
        private static extern int OCIServerAttach(void* srvhp, void* errhp, void* dblink, int dblink_len, uint mode);
        public static int ServerAttach(IntPtr srvhp, IntPtr errhp, string dblink, int dblink_len, uint mode)
        {
            IntPtr dblinkPtr = IntPtr.Zero;
            try
            {
                dblinkPtr = Marshal.StringToHGlobalAnsi(dblink);
                return OCIServerAttach(srvhp.ToPointer(), errhp.ToPointer(), dblinkPtr.ToPointer(), dblink_len, mode);
            }
            finally
            {
                if (dblinkPtr != IntPtr.Zero) Marshal.FreeHGlobal(dblinkPtr);
            }
        }

        // -- OCISessionBegin
        [DllImport("oci.dll", EntryPoint = "OCISessionBegin")]
        private static extern int OCISessionBegin(void* svchp, void* errhp, void* usrhp, uint credt, uint mode);
        public static int SessionBegin(IntPtr svchp, IntPtr errhp, IntPtr usrhp, uint credt, uint mode)
        {
            return OCISessionBegin(svchp.ToPointer(), errhp.ToPointer(), usrhp.ToPointer(), credt, mode);
        }

        // -- OCIStmtExecute
        [DllImport("oci.dll", EntryPoint = "OCIStmtExecute")]
        private static extern int OCIStmtPrepare(void* svchp, void* stmtp, void* errhp, uint iters, uint rowoff, void* snap_in, void* snap_out, uint mode);
        public static int StmtExecute(IntPtr svchp, IntPtr stmtp, IntPtr errhp, uint iters, uint rowoff, IntPtr snap_in, IntPtr snap_out, uint mode)
        {
            return OCIStmtPrepare(svchp.ToPointer(), stmtp.ToPointer(), errhp.ToPointer(), iters, rowoff, snap_in.ToPointer(), snap_out.ToPointer(), mode);
        }

        // -- OCIStmtFetch2
        [DllImport("oci.dll", EntryPoint = "OCIStmtFetch2")]
        private static extern int OCIStmtFetch2(void* stmtp, void* errhp, uint nrows, ushort orientation, uint scrollOffset, uint mode);
        public static int StmtFetch2(IntPtr stmtp, IntPtr errhp, uint nrows, ushort orientation, uint scrollOffset, uint mode)
        {
                return OCIStmtFetch2(stmtp.ToPointer(), errhp.ToPointer(), nrows, orientation, scrollOffset, mode);
        }

        // -- OCIStmtPrepare
        [DllImport("oci.dll", EntryPoint = "OCIStmtPrepare")]
        private static extern int OCIStmtPrepare(void* stmtp, void* errhp, void* stmt, uint stmt_len, uint language, uint mode);
        public static int StmtPrepare(IntPtr stmtp, IntPtr errhp, string stmt, uint stmt_len, uint language, uint mode)
        {
            IntPtr stmtPtr = IntPtr.Zero;
            try
            {
                stmtPtr = Marshal.StringToHGlobalAnsi(stmt);
                return OCIStmtPrepare(stmtp.ToPointer(), errhp.ToPointer(), stmtPtr.ToPointer(), stmt_len, language, mode);
            }
            finally
            {
                if (stmtPtr != IntPtr.Zero) Marshal.FreeHGlobal(stmtPtr);
            }
        }

        #endregion
    }
}
