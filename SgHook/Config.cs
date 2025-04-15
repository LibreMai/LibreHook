using System.IO;
using YamlDotNet.Serialization;

namespace SgHook
{
    public class Config
    {
        private static string configFilePath = "sghook.yaml";

        private static SgHookConfig _conf;
        public static SgHookConfig Conf
        {
            set { _conf = value; }
            get { return _conf; }
        }

        public static void InitConfigFile() {
            var configFile = File.ReadAllText(configFilePath);
            var deserializer = new DeserializerBuilder().Build();
            _conf = deserializer.Deserialize<SgHookConfig>(configFile);
        }
    }
    public class NetConfigOverrideConfig
    {
        public bool filterUnCompressed { get; set; }
        public bool enable {  get; set; }
        public string moduleName { get; set; }
    }
    public class SgHookConfig
    {
        public string greeting { get; set; }
        public MiscConfig misc { get; set; }

#if NETPACK
        public NetPackConfig netPack { get; set; }
#endif
        public BypassCheckConfig bypassCheck { get; set; }
#if OVERRIDEPOWERON
        public OverridePowerOnConfig overridePowerOn { get; set; }
#endif
        public CameraHookConfig cameraHook { get; set; }
        public JudgeOverrideConfig judgeOverride { get; set; }
        public OnProgramStartConfig onProgramStart { get; set; }
        public InputRestoreConfig inputRestore { get; set; }
#if AUTOPLAY
        public AutoPlayConfig autoPlay { get; set; }
#endif
        public UselessConfig useless { get; set; }
        public NetConfigOverrideConfig netConfigOverride { get; set; }
    }
    public class UselessConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool hideDerakKumaHead { get; set; }
    }
    public class MiscConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public string reloadKey { get; set; }
        public bool fixParticle { get; set; }
        public bool eventOverride { get; set; }
    }
#if AUTOPLAY
    public class AutoPlayConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public string switchKey { get; set; }
        public uint criticalRate { get; set; }
        public uint perfectRate { get; set; }
        public uint perfect2ndRate { get; set; }
        public uint greatRate { get; set; }
        public uint goodRate { get; set; }
        public uint fastRate { get; set; }
    }
#endif
#if NETPACK
    public class NetPackConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool debugRequest { get; set; }
        public bool debugResponse { get; set; }
        public bool debugUrl { get; set; }
        public string overrideBaseUrl { get; set; }
        public bool disableObEnc { get; set; }
        public string removeSuffixRegex { get; set; }
        public string removeUrlRegex { get; set; }
    }
#endif
    public class BypassCheckConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public int magicNumber { get; set; }
        public bool disableIniFileClear { get; set; }
        public bool forceAsServer { get; set; }
        public bool forceIgnoreError { get; set; }
        public uint keepCredits { get; set; }
        public bool disableMaintenance { get; set; }
        public int waitTime { get; set; }
        public bool disableAutoReboot { get; set; }
        public bool forceEnableQuickRetry { get; set; }
        public bool fixQuickRetryFastLateBug { get; set; }
        public int rewriteTitleRouterConfigLog { get; set; }
        public bool bypassCheckServerHash { get; set; }
    }
#if OVERRIDEPOWERON
    public class OverridePowerOnConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool overrideAllNetIsGood { get; set; }
        public bool overrideNetworkTest { get; set; }

        public bool dumpPowerOnProcess { get; set; }
    }
#endif
    public class CameraHookConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool playerPhotoCameraScanQR { get; set; }
        public bool skipWebCamTextureInitLimit { get; set; }
        public bool fixQRCodeScanResLimit { get; set; }
        
        public bool hookCameraInitialize { get; set; }
        public bool showCamerasList { get; set; }
        public int specifyCameraPhoto { get; set; }
        public int specifyCameraChime { get; set; }
        public int selfCheckMilliSeconds { get; set; }
    }
    public class JudgeOverrideConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public float offsetA { get; set; }
        public float offsetB { get; set; }
        public float musicVolume { get; set; }
        public int vSyncCount { get; set; }
        public float particleRate { get; set; }
        public bool wasapiExclusive { get; set; }
        public int touchPanelDelay { get; set; }
    }
    public class OnProgramStartConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool probeAllSensitive { get; set; }

        public bool singlePlayer { get; set; }

    }
    public class InputRestoreConfig
    {
        public bool enable { get; set; }
        public string moduleName { get; set; }
        public bool restoreDebugInput { get; set; }
    }
}
