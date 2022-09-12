using HandySharp.HandyPutRequest;
using HandySharp.HandyResult;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace HandySharp
{
    /// <summary>
    /// Main class for the Handy.
    /// For more information on the different API endpoints please view: https://handyfeeling.com/api/handy/v2/docs/ 
    /// </summary>
    public class Handy
    {
        private static readonly HttpClient client = new HttpClient();

        private string apiUrl;
        /// <summary>
        /// Indicates if the handy is connected with the server.
        /// </summary>
        public bool Connected { get; private set; }
        private string connectionKey;
        private InfoResult? deviceInfo;
        private int majorVersion;
        private int minorVersion;
        private int currentMode;
        /// <summary>
        /// Holds the offset between the application and the server.
        /// </summary>
        public long ClientServerOffset { get; private set; }

        /// <summary>
        /// Initialize a new instance connecting to the specified API endpoint.
        /// Use this for non-standard API servers.
        /// </summary>
        /// <param name="apiUrl">A <see cref="System.String"/> value representing the url the application will connect with.</param>
        public Handy(string apiUrl)
        {            
            Init(apiUrl);
        }

        /// <summary>
        /// Initialize a new instance connecting to the standard API endpoint.
        /// </summary>
        public Handy()
        {
            Init("https://www.handyfeeling.com/api/handy/v2");
        }

        private void Init(string apiUrl)
        {
            this.apiUrl = apiUrl;
            this.Connected = false;
            this.connectionKey = "";
            this.deviceInfo = null;
            this.majorVersion = 0;
            this.minorVersion = 0;
            this.currentMode = -1;
        }

        /// <summary>
        /// Establishes a proper connection to the handy using a connection key.
        /// </summary>
        /// <param name="connectionKey">The <see cref="System.String"/> instance representing a connection key</param>
        /// <returns>An instance of the <see cref="Result{T}"/> class indicating if a connection could be established or not.
        /// Provides also device information to determine further details.</returns>
        public async Task<Result<InfoResult>> Connect(string connectionKey)
        {
            this.connectionKey = connectionKey;

            ConnectedResult c = await GetConnected();

            if (c.Connected)
            {
                InfoResult i = await GetInfo();

                if(i.FwStatus == 0)
                {
                    deviceInfo = i;
                    string[] FWVersionSplit = deviceInfo.FwVersion.Split('.');
                    majorVersion = int.Parse(FWVersionSplit[0]);
                    minorVersion = int.Parse(FWVersionSplit[1]);
                    Connected = true;                    
                }
                else
                {
                    this.connectionKey = "";
                    Connected = false;                    
                }

                return new Result<InfoResult>(Connected, i);
            }

            this.connectionKey = "";
            return new Result<InfoResult>(false, null);
        }

        /// <summary>
        /// Disconnects the application from the handy.
        /// </summary>
        /// <returns>True if the device is now disconnected and false if no device was connected.</returns>
        public bool Disconnect()
        {
            if(Connected)
            {
                Connected = false;
                connectionKey = "";
                deviceInfo = null;
                majorVersion = 0;
                minorVersion = 0;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Fetches the current status of the handy.
        /// </summary>
        /// <returns>A <see cref="StatusResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<StatusResult> GetStatus()
        {
            CheckConnected();

            StatusResult result = await GetResponse<StatusResult>(ApiEndpoints.STATUS);

            if(currentMode != result.Mode)
            {
                currentMode = result.Mode;
            }

            return result;
        }

        /// <summary>
        /// Fetches the current settings of the handy.
        /// </summary>
        /// <returns>A <see cref="SettingsResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<SettingsResult> GetSettings()
        {
            CheckConnected();

            return await GetResponse<SettingsResult>(ApiEndpoints.SETTINGS);
        }

        /// <summary>
        /// Fetches the current mode of the handy.
        /// </summary>
        /// <returns>A <see cref="ModeResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<ModeResult> GetMode()
        {
            CheckConnected();

            ModeResult result = await GetResponse<ModeResult>(ApiEndpoints.MODE);

            if(currentMode != result.Mode)
            {
                currentMode = result.Mode;
            }

            return result;
        }

        /// <summary>
        /// Sets the mode of the connected handy.
        /// </summary>
        /// <param name="mode">One of the <see cref="Modes"/> the handy can be put into</param>
        /// <returns>A <see cref="ModeResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<SetModeResult> SetMode(Modes mode)
        {
            CheckConnected();

            ModeRequest modeRequest = new ModeRequest(mode);
            SetModeResult result = await PutResponse<SetModeResult>(ApiEndpoints.MODE, modeRequest);

            if(result.Result != -1 && currentMode != (int)mode)
            {
                currentMode = (int)mode;
            }

            return result;
        }

        /// <summary>
        /// Starts the motion of the slider of the connected handy. Does it only if the handy is set in HAMP mode.
        /// </summary>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHAMPStart()
        {
            CheckConnected();
 
            CheckMode(Modes.HAMP);

            return await PutResponse<GenericResult>(ApiEndpoints.HAMP_START, null);
        }

        /// <summary>
        /// Stops the motion of the slider of the connected handy. Does it only if the handy is set in HAMP mode.
        /// </summary>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHAMPStop()
        {
            CheckConnected();

            CheckMode(Modes.HAMP);

            return await PutResponse<GenericResult>(ApiEndpoints.HAMP_STOP, null);
        }

        /// <summary>
        /// Fetches the velocity of the connected handy. Does it only if the handy is set in HAMP mode.
        /// </summary>
        /// <returns>A <see cref="HAMPVelocityResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HAMPVelocityResult> GetHAMPVelocity()
        {
            CheckConnected();

            CheckMode(Modes.HAMP);

            return await GetResponse<HAMPVelocityResult>(ApiEndpoints.HAMP_VELOCITY);
        }

        /// <summary>
        /// Sets the desired velocity of the connected handy. Does it only if the handy is set in HAMP mode.
        /// </summary>
        /// <param name="velocity">A <see cref="System.Double"/> value indicating the percent (0-100) from the maximum velocity of the handy. 
        /// Parameter is clamped between 0 and 100.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHAMPVelocity(double velocity)
        {
            CheckConnected();

            CheckMode(Modes.HAMP);

            HAMPStateResult state = await GetHAMPState();

            if(state.Result == -1 || state.State != 2)
            {
                throw new HandyErrorException(GetHAMPError());
            }

            VelocityRequest vel = new VelocityRequest();
            vel.Velocity = velocity;

            return await PutResponse<GenericResult>(ApiEndpoints.HAMP_VELOCITY, vel);
        }

        /// <summary>
        /// Fetches the state of the connected handy. Does it only if the handy is set in HAMP mode.
        /// </summary>
        /// <returns>A <see cref="HAMPStateResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HAMPStateResult> GetHAMPState()
        {
            CheckConnected();

            CheckMode(Modes.HAMP);

            return await GetResponse<HAMPStateResult>(ApiEndpoints.HAMP_STATE);
        }

        /// <summary>
        /// Sets the next desired position and velocity for the connected handy. Does it only if the handy is set in HDSP mode.
        /// </summary>
        /// <param name="position">A <see cref="System.Double"/> value indicating the absolute position of the slider.</param>
        /// <param name="velocity">A <see cref="System.Int32"/> value indicating the absolute speed in mm/ms.</param>
        /// <param name="stopOnTarget">A <see cref="System.Boolean"/> indicating if the slide should stop once the position is received or continue with the next instruction without delay.</param>
        /// <param name="immediateResponse">A <see cref="System.Boolean"/> indicating if the server should send a response without waiting of the handy to finish the command.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHDSPxava(double position, int velocity, bool stopOnTarget = false, bool immediateResponse = false)
        {
            CheckConnected();

            CheckMode(Modes.HDSP);

            HDSPVelocityRequest vel = new HDSPVelocityRequest();
            vel.StopOnTarget = stopOnTarget;
            vel.ImmediateResponse = immediateResponse;
            vel.Position = position;
            vel.Velocity = velocity;

            return await PutResponse<GenericResult>(ApiEndpoints.HDSP_XAVA, vel);
        }

        /// <summary>
        /// Sets the next desired position and velocity for the connected handy. Does it only if the handy is set in HDSP mode.
        /// </summary>
        /// <param name="position">A <see cref="System.Double"/> value indicating the relative position of the slider in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="velocity">A <see cref="System.Int32"/> value indicating the absolute speed in mm/ms.</param>
        /// <param name="stopOnTarget">A <see cref="System.Boolean"/> indicating if the slide should stop once the position is received or continue with the next instruction without delay.</param>
        /// <param name="immediateResponse">A <see cref="System.Boolean"/> indicating if the server should send a response without waiting of the handy to finish the command.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHDSPxpva(double position, int velocity, bool stopOnTarget = false, bool immediateResponse = false)
        {
            CheckConnected();

            CheckMode(Modes.HDSP);

            HDSPVelocityRequest vel = new HDSPVelocityRequest();
            vel.StopOnTarget = stopOnTarget;
            vel.ImmediateResponse = immediateResponse;
            vel.Position = Math.Clamp(position, 0, 100);
            vel.Velocity = velocity;

            return await PutResponse<GenericResult>(ApiEndpoints.HDSP_XPVA, vel);
        }

        /// <summary>
        /// Sets the next desired position and velocity for the connected handy. Does it only if the handy is set in HDSP mode.
        /// </summary>
        /// <param name="position">A <see cref="System.Double"/> value indicating the relative position of the slider in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="velocity">A <see cref="System.Double"/> value indicating the relative speed in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="stopOnTarget">A <see cref="System.Boolean"/> indicating if the slide should stop once the position is received or continue with the next instruction without delay.</param>
        /// <param name="immediateResponse">A <see cref="System.Boolean"/> indicating if the server should send a response without waiting of the handy to finish the command.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHDSPxpvp(double position, double velocity, bool stopOnTarget = false, bool immediateResponse = false)
        {
            CheckConnected();

            CheckMode(Modes.HDSP);

            HDSPVelocityRequest vel = new HDSPVelocityRequest();
            vel.StopOnTarget = stopOnTarget;
            vel.ImmediateResponse = immediateResponse;
            vel.Position = Math.Clamp(position, 0, 100);
            vel.Velocity = Math.Clamp(velocity, 0, 100);

            return await PutResponse<GenericResult>(ApiEndpoints.HDSP_XPVP, vel);
        }

        /// <summary>
        /// Sets the next desired position and duration for the connected handy. Does it only if the handy is set in HDSP mode.
        /// </summary>
        /// <param name="position">A <see cref="System.Double"/> value indicating the absolute position of the slider.</param>
        /// <param name="duration">A <see cref="System.Int32"/> value indicating the absolute duration in milliseconds until the position is reached.</param>
        /// <param name="stopOnTarget">A <see cref="System.Boolean"/> indicating if the slide should stop once the position is received or continue with the next instruction without delay.</param>
        /// <param name="immediateResponse">A <see cref="System.Boolean"/> indicating if the server should send a response without waiting of the handy to finish the command.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHDSPxat(double position, int duration, bool stopOnTarget = false, bool immediateResponse = false)
        {
            CheckConnected();

            CheckMode(Modes.HDSP);

            HDSPDurationRequest vel = new HDSPDurationRequest();
            vel.StopOnTarget = stopOnTarget;
            vel.ImmediateResponse = immediateResponse;
            vel.Position = position;
            vel.Duration = duration;

            return await PutResponse<GenericResult>(ApiEndpoints.HDSP_XAT, vel);
        }

        /// <summary>
        /// Sets the next desired position and duration for the connected handy. Does it only if the handy is set in HDSP mode.
        /// </summary>
        /// <param name="position">A <see cref="System.Double"/> value indicating the relative position of the slider in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="duration">A <see cref="System.Int32"/> value indicating the absolute duration in milliseconds until the position is reached.</param>
        /// <param name="stopOnTarget">A <see cref="System.Boolean"/> indicating if the slide should stop once the position is received or continue with the next instruction without delay.</param>
        /// <param name="immediateResponse">A <see cref="System.Boolean"/> indicating if the server should send a response without waiting of the handy to finish the command.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHDSPxpt(double position, int duration, bool stopOnTarget = false, bool immediateResponse = false)
        {
            CheckConnected();

            CheckMode(Modes.HDSP);

            HDSPDurationRequest vel = new HDSPDurationRequest();
            vel.StopOnTarget = stopOnTarget;
            vel.ImmediateResponse = immediateResponse;
            vel.Position = Math.Clamp(position, 0, 100);
            vel.Duration = duration;

            return await PutResponse<GenericResult>(ApiEndpoints.HDSP_XAT, vel);
        }

        /// <summary>
        /// Starts playing a script from a certain time and synced to the server. Does it only if the handy is set in HSSP mode.
        /// </summary>
        /// <param name="estimatedServerTime">The estimated time of the server. This is needed for synchronization. See <see cref="Handy.ClientServerOffset"/> for an estimation.</param>
        /// <param name="startTime">A <see cref="System.Int64"/> value representing the milliseconds since script start.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHSSPPlay(long estimatedServerTime, long startTime)
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            HSSPPlayRequest play = new HSSPPlayRequest();
            play.EstimatedServerTime = estimatedServerTime;
            play.StartTime = startTime;

            return await PutResponse<GenericResult>(ApiEndpoints.HSSP_PLAY, play);
        }

        /// <summary>
        /// Stops playing the current script. Does it only if the handy is set in HSSP mode.
        /// </summary>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHSSPStop()
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            return await PutResponse<GenericResult>(ApiEndpoints.HSSP_STOP, null);
        }

        /// <summary>
        /// Downloads a script from the provided url to the connected handy. Does it only if the handy is set in HSSP mode.
        /// </summary>
        /// <param name="url">A <see cref="System.String"/> containing the url to a script</param>
        /// <param name="sha256">A <see cref="System.String"/> containing a hash. If provided the device checks if the script was already downloaded before.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHSSPSetup(string url, string? sha256 = null)
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            HSSPSetupRequest request = new HSSPSetupRequest();
            request.Url = url;
            request.Sha256 = sha256;

            return await PutResponse<GenericResult>(ApiEndpoints.HSSP_SETUP, request);
        }

        /// <summary>
        /// Fetches if the handy loops the current playing script once it's end is reached. Does it only if the handy is set in HSSP mode. 
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>
        /// <returns>A <see cref="HSSPLoopResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSSPLoopResult> GetHSSPLoop()
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            CheckDeviceVersion(3, 2);

            return await GetResponse<HSSPLoopResult>(ApiEndpoints.HSSP_LOOP);
        }

        /// <summary>
        /// Sets if the handy shall loop the current playing script once it's end is reached. Does it only if the handy is set in HSSP mode. 
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>
        /// <param name="looping">A <see cref="System.Boolean"/> indicating if the script shall be looped or not.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHSSPLoop(bool looping)
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            CheckDeviceVersion(3, 2);

            HSSPLoopRequest request = new HSSPLoopRequest();
            request.Activated = looping;

            return await PutResponse<GenericResult>(ApiEndpoints.HSSP_LOOP, request);
        }

        /// <summary>
        /// Fetches the state of the connected handy regarding to the HSSP mode. Does it only if the handy is set in HSSP mode. 
        /// </summary>        
        /// <returns>A <see cref="HSSPStateResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSSPStateResult> GetHSSPState()
        {
            CheckConnected();

            CheckMode(Modes.HSSP);

            return await GetResponse<HSSPStateResult>(ApiEndpoints.HSSP_STATE);
        }

        /// <summary>
        /// Fetches the time of the device. If synchronized it is the estimated server time. Does it only if the handy is set in HSTP mode.
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>        
        /// <returns>A <see cref="HSTPDeviceTimeResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSTPDeviceTimeResult> GetHSTPTime()
        {
            CheckConnected();

            CheckDeviceVersion(3, 2);

            return await GetResponse<HSTPDeviceTimeResult>(ApiEndpoints.HSTP_TIME);
        }

        /// <summary>
        /// Fetches the currently set offset between server time and client time. Does it only if the handy is set in HSTP mode.
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>        
        /// <returns>A <see cref="HSTPOffsetResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSTPOffsetResult> GetHSTPOffset()
        {
            CheckConnected();

            CheckDeviceVersion(3, 2);

            return await GetResponse<HSTPOffsetResult>(ApiEndpoints.HSTP_OFFSET);
        }

        /// <summary>
        /// Sets the offset between server time and client time. Does it only if the handy is set in HSTP mode.
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>   
        /// <param name="offset">A <see cref="System.Int32"/> value representing the offset in milliseconds.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetHSTPOffset(int offset)
        {
            CheckConnected();

            CheckDeviceVersion(3, 2);

            OffsetRequest offsetRequest = new OffsetRequest();
            offsetRequest.Offset = offset;

            return await PutResponse<GenericResult>(ApiEndpoints.HSTP_OFFSET, offsetRequest);
        }

        /// <summary>
        /// Fetches the round trip delay between server and device. Does it only if the handy is set in HSTP mode.
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>
        /// <returns>A <see cref="HSTPRtdResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSTPRtdResult> GetHSTPRtd()
        {
            CheckConnected();

            CheckDeviceVersion(3, 2);

            return await GetResponse<HSTPRtdResult>(ApiEndpoints.HSTP_RTD);
        }

        /// <summary>
        /// Snychronizes the connected handy with the server. Does it only if the handy is set in HSTP mode.
        /// Needs a device with firmware 3.2.x to be available.
        /// </summary>
        /// <param name="syncCount">A <see cref="System.Int32"/> value representing the number of round trip samples to take. Default: 30</param>
        /// <param name="outliers">A <see cref="System.Int32"/> value indicating how many results can be declared as outliers. Default: 6</param>
        /// <returns>A <see cref="HSTPSyncResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<HSTPSyncResult> HSTPSync(int syncCount = 30, int outliers = 6)
        {
            CheckConnected();

            CheckDeviceVersion(3, 2);
            
            HttpRequestMessage msg = CreateGetRequest(ApiEndpoints.HSTP_SYNC + "?syncCount=" + syncCount + "&outliers=" + outliers);

            HttpResponseMessage response = await client.SendAsync(msg);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultString = await response.Content.ReadAsStringAsync();

                HSTPSyncResult? result = JsonConvert.DeserializeObject<HSTPSyncResult>(resultString);

                if (result == null)
                {
                    ErrorResult? e = JsonConvert.DeserializeObject<ErrorResult>(resultString);

                    throw new HandyErrorException(e.Error);
                }

                return result;
            }
            else
            {
                throw ThrowHttpException(response);
            }
        }

        /// <summary>
        /// Fetches the currently set limits to the slider positions of the connected handy.
        /// </summary>
        /// <returns>A <see cref="SlideResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<SlideResult> GetSlide()
        {
            CheckConnected();

            return await GetResponse<SlideResult>(ApiEndpoints.SLIDE);
        }

        /// <summary>
        /// Sets the limits of the slider positions of the connected handy.
        /// </summary>
        /// <param name="min">A <see cref="System.Double"/> value indicating the minimum position in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="max">A <see cref="System.Double"/> value indicating the maximum position in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetSlide(double min, double max)
        {
            CheckConnected();

            SliderMinMaxRequest minMaxRequest = new SliderMinMaxRequest();
            minMaxRequest.Min = min;
            minMaxRequest.Max = max;

            return await PutResponse<GenericResult>(ApiEndpoints.SLIDE, minMaxRequest);
        }

        /// <summary>
        /// Sets the limits of the slider positions of the connected handy.
        /// </summary>
        /// <param name="useMax">A <see cref="System.Boolean"/> value indicating if the maximum (true) or minimum (false) value should be adjusted.</param>
        /// <param name="value">A <see cref="System.Double"/> value indicating the new position in percent (0-100). Parameter is clamped between 0 and 100.</param>
        /// <param name="shift">A <see cref="System.Boolean"/> optional value indicating if the <paramref name="value"/> should be treated as shifting amount.</param>
        /// <returns>A <see cref="GenericResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<GenericResult> SetSlide(bool useMax, double value, bool shift = false)
        {
            CheckConnected();

            if (useMax)
            {
                SliderMaxRequest maxRequest = new SliderMaxRequest();
                maxRequest.Max = value;
                maxRequest.Fixed = shift;

                return await PutResponse<GenericResult>(ApiEndpoints.SLIDE, maxRequest);
            }
            else
            {
                SliderMinRequest minRequest = new SliderMinRequest();
                minRequest.Min = value;
                minRequest.Fixed = shift;

                return await PutResponse<GenericResult>(ApiEndpoints.SLIDE, minRequest);
            }
        }

        /// <summary>
        /// Fetches the position of the slide of the connected handy.
        /// </summary>
        /// <returns>A <see cref="SlidePositionAbsoluteResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<SlidePositionAbsoluteResult> GetSlidePositionAbsolute()
        {
            CheckConnected();

            return await GetResponse<SlidePositionAbsoluteResult>(ApiEndpoints.SLIDE_POSITION_ABSOLUTE);
        }

        /// <summary>
        /// Calculates the offset between server and client. The offset will be available in <see cref="Handy.ClientServerOffset"/> afterwards.
        /// </summary>
        /// <param name="samples">A <see cref="System.Int32"/> value representing the number of samples taken for the calculation. Default: 30</param>
        /// <returns>The calculated client server offset.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<long> CalculateClientServerOffset(int samples = 30)
        {
            long offset_agg = 0;
            long successful_samples = 0;

            for(int i=0;i<samples;i++)
            {
                try
                {
                    (long, long, ServertimeResult?) result = await GetServertime();

                    if (result.Item3 != null)
                    {
                        long T_est = result.Item3.Servertime + result.Item1 / 2;
                        long offset = T_est - result.Item2;
                        offset_agg += offset;
                        successful_samples++;
                    }
                    else
                    {
                        i--;
                    }
                }
                catch(HandyHttpException e)
                {
                    if(e.StatusCode == HttpStatusCode.BadRequest)
                    {
                        await Task.Delay((int)e.Milliseconds);
                        i--;
                    }
                }
            }

            ClientServerOffset = (long)((double)offset_agg / (double)successful_samples); 
            return ClientServerOffset;
        }

        /// <summary>
        /// Fetches the servertime of the current set server.
        /// </summary>
        /// <returns>A triple indicating the round trip time, the clients time once the response was received and a <see cref="ServertimeResult"/> object containing the server time.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        private async Task<(long, long, ServertimeResult?)> GetServertime()
        {
            long rtt = 0;
            long clientTime = 0;
            HttpRequestMessage msg = CreateGetRequest(ApiEndpoints.SERVERTIME);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpResponseMessage response = await client.SendAsync(msg);
            sw.Stop();
            clientTime = DateTime.Now.Ticks;

            rtt = sw.ElapsedTicks;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultString = await response.Content.ReadAsStringAsync();

                ServertimeResult? result = JsonConvert.DeserializeObject<ServertimeResult>(resultString);

                return (rtt, clientTime, result);
            }
            else
            {
                throw ThrowHttpException(response);
            }
        }

        /// <summary>
        /// Fetches if the handy is connected to the server.
        /// </summary>
        /// <returns>A <see cref="ConnectedResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        private async Task<ConnectedResult> GetConnected()
        {
            return await GetResponse<ConnectedResult>(ApiEndpoints.CONNECTED);
        }

        /// <summary>
        /// Fetches information from the connected handy.
        /// </summary>
        /// <returns>A <see cref="InfoResult"/> object.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        public async Task<InfoResult> GetInfo()
        { 
            return await GetResponse<InfoResult>(ApiEndpoints.INFO);
        }

        /// <summary>
        /// Sends a GET request to the <paramref name="apiEndpoint"/> and parses the response into the specified object.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> representing the endpoint of the API.</param>
        /// <returns>An object of specified type.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        private async Task<T> GetResponse<T>(string apiEndpoint)
        {
            HttpRequestMessage msg = CreateGetRequest(apiEndpoint);

            HttpResponseMessage response = await client.SendAsync(msg);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultString = await response.Content.ReadAsStringAsync();

                T? result = JsonConvert.DeserializeObject<T>(resultString);

                if (result == null)
                {
                    ErrorResult? e = JsonConvert.DeserializeObject<ErrorResult>(resultString);

                    throw new HandyErrorException(e.Error);
                }

                return result;
            }
            else
            {
                throw ThrowHttpException(response);
            }
        }

        /// <summary>
        /// Sends a PUT request to the <paramref name="apiEndpoint"/> and parses the response into the specified object.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> representing the endpoint of the API.</param>
        /// <returns>An object of specified type.</returns>
        /// <exception cref="HandyErrorException">
        /// If something with the device is not as expected.
        /// </exception>
        /// <exception cref="HandyHttpException">
        /// If the http response does not return <see cref="HttpStatusCode.OK"/>
        /// </exception>
        private async Task<T> PutResponse<T>(string apiEndpoint, object body)
        {
            HttpRequestMessage msg = CreatePutRequest(apiEndpoint, body);

            HttpResponseMessage response = await client.SendAsync(msg);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultString = await response.Content.ReadAsStringAsync();

                T? result = JsonConvert.DeserializeObject<T>(resultString);

                if (result == null)
                {
                    ErrorResult? e = JsonConvert.DeserializeObject<ErrorResult>(resultString);

                    throw new HandyErrorException(e.Error);
                }

                return result;
            }
            else
            {
                throw ThrowHttpException(response);
            }
        }

        /// <summary>
        /// Creates a Device Not Connected Error.
        /// </summary>
        /// <returns>A custom <see cref="Error"/> object.</returns>
        private Error GetDeviceNotConnectedError()
        {
            Error e = new Error();
            e.Code = 1001;
            e.Name = "DeviceNotConnected";
            e.Message = "Device not connected";
            e.Connected = Connected;

            return e;
        }

        /// <summary>
        /// Creates a Device Version Number Error.
        /// </summary>
        /// <returns>A custom <see cref="Error"/> object.</returns>
        private Error GetDeviceVersionNumberError()
        {
            Error e = new Error();
            e.Code = 999;
            e.Name = "WrongDeviceVersion";
            e.Message = "Device Version does not met requirements";
            e.Connected = Connected;

            return e;
        }

        /// <summary>
        /// Creates a Wrong Mode Error.
        /// </summary>
        /// <returns>A custom <see cref="Error"/> object.</returns>
        private Error GetWrongModeError()
        {
            Error e = new Error();
            e.Code = 666;
            e.Name = "WrongMode";
            e.Message = "Mode is not set to the right one";
            e.Connected = Connected;

            return e;
        }

        /// <summary>
        /// Creates a HAMPError.
        /// </summary>
        /// <returns>A custom <see cref="Error"/> object.</returns>
        private Error GetHAMPError()
        {
            Error e = new Error();
            e.Code = 3000;
            e.Name = "HampError";
            e.Message = "HampError";
            e.Connected = Connected;

            return e;
        }

        /// <summary>
        /// Checks if the handy is and the application are connected.
        /// </summary>
        /// <exception cref="HandyErrorException">
        /// If the device is not connected a device not connected error is thrown.
        /// </exception>
        private void CheckConnected()
        {
            if (!Connected)
            {
                throw new HandyErrorException(GetDeviceNotConnectedError());
            }
        }

        /// <summary>
        /// Checks if the device is currently in the desired mode <paramref name="m"/>. Fetches the mode of the device if it wasn't already.
        /// </summary>
        /// <param name="m">One of the different <see cref="Modes"/> the handy can be set into.</param>
        /// <exception cref="HandyErrorException">
        /// If the mode is not the desired one.
        /// </exception>
        private async void CheckMode(Modes m)
        {
            if (currentMode == -1)
            {
                ModeResult mode = await GetMode();

                if (mode.Mode != (int)m)
                {
                    throw new HandyErrorException(GetWrongModeError());
                }
            }
            else if(currentMode != (int)m)
            {
                throw new HandyErrorException(GetWrongModeError());
            }
        }

        /// <summary>
        /// Checks for the specified device version.
        /// </summary>
        /// <param name="major">A <see cref="System.Int32"/> value representing the major version.</param>
        /// <param name="minor">A <see cref="System.Int32"/> value representing the minor version.</param>
        /// <exception cref="HandyErrorException">
        /// If the device version is lower than the specified one or the device information was not requested already.
        /// </exception>
        private void CheckDeviceVersion(int major, int minor)
        {
            if (deviceInfo == null || (majorVersion < major && minorVersion < minor))
            {
                throw new HandyErrorException(GetDeviceVersionNumberError());
            }
        }

        /// <summary>
        /// Builds a <see cref="HandyHttpException"/> out of a <see cref="HttpResponseMessage"/> <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> that needs to be converted.</param>
        /// <returns>A <see cref="HandyHttpException"/> object.</returns>
        private HandyHttpException ThrowHttpException(HttpResponseMessage response)
        {
            IEnumerable<string>? reset;
            if (response.Headers.TryGetValues("X-RateLimit-Reset", out reset))
            {
                int milli = int.Parse(reset.First());

                return new HandyHttpException(response.StatusCode, milli);
            }
            else
            {
                return new HandyHttpException(response.StatusCode);
            }
        }

        /// <summary>
        /// Creates a proper GET request for the API to work with.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> value that represents the endpoint of the API.</param>
        /// <returns>A <see cref="HttpRequestMessage"/> object.</returns>
        private HttpRequestMessage CreateGetRequest(string apiEndpoint)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Connection-Key", connectionKey);

            return CreateGetRequest(apiEndpoint, headers);
        }

        /// <summary>
        /// Creates a proper GET request for the API to work with.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> value that represents the endpoint of the API.</param>
        /// <param name="header">A <see cref="System.Collections.Generic.Dictionary{System.String, System.String}"/> containing various headers</param>
        /// <returns>A <see cref="HttpRequestMessage"/> object.</returns>
        private HttpRequestMessage CreateGetRequest(string apiEndpoint, Dictionary<string, string> header)
        {
            return CreateRequest(HttpMethod.Get, apiEndpoint, header, null);
        }

        /// <summary>
        /// Creates a proper PUT request for the API to work with.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> value that represents the endpoint of the API.</param>
        /// <param name="body">A <see cref="object"/> that will be parsed as JSON</param>
        /// <returns>A <see cref="HttpRequestMessage"/> object.</returns>
        private HttpRequestMessage CreatePutRequest(string apiEndpoint, object body)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Connection-Key", connectionKey);

            return CreatePutRequest(apiEndpoint, headers, body);
        }

        /// <summary>
        /// Creates a proper PUT request for the API to work with.
        /// </summary>
        /// <param name="apiEndpoint">A <see cref="System.String"/> value that represents the endpoint of the API.</param>
        /// <param name="header">A <see cref="System.Collections.Generic.Dictionary{System.String, System.String}"/> containing various headers</param>
        /// <param name="body">A <see cref="object"/> that will be parsed as JSON</param>
        /// <returns>A <see cref="HttpRequestMessage"/> object.</returns>
        private HttpRequestMessage CreatePutRequest(string apiEndpoint, Dictionary<string, string> header, object body)
        {
            return CreateRequest(HttpMethod.Put, apiEndpoint, header, body);
        }

        /// <summary>
        /// Creates a proper request for the API to work with.
        /// </summary>
        /// <param name="method">A <see cref="HttpMethod"/> value indicating the method used for the request.</param>
        /// <param name="apiEndpoint">A <see cref="System.String"/> value that represents the endpoint of the API.</param>
        /// <param name="header">A <see cref="System.Collections.Generic.Dictionary{System.String, System.String}"/> containing various headers</param>
        /// <param name="body">A <see cref="object"/> that will be parsed as JSON</param>
        /// <returns>A <see cref="HttpRequestMessage"/> object.</returns>
        private HttpRequestMessage CreateRequest(HttpMethod method, string apiEndpoint, Dictionary<string, string> header, object? body)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            msg.RequestUri = new Uri(apiUrl + "/" + apiEndpoint);
            msg.Method = method;
            msg.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            foreach (string headerName in header.Keys)
            {
                msg.Headers.Add(headerName, header[headerName]);
            }

            if (body != null)
            {
                string content = JsonConvert.SerializeObject(body);
                StringContent msgContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                msg.Content = msgContent;
            }

            return msg;
        }
    }
}