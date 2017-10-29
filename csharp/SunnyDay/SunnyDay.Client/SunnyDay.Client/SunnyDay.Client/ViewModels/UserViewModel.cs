using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using SunnyDay.Client.Core.Utils.DataConvert;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private ICloudService _service;
        private readonly IBandService _band;
        private readonly IWeatherService _weather;
        private bool _fullTimeFormat;

        public User UserItem { get; private set; }
        private Alarm _alarm;
        private TimeSpan _timer;
        private bool _oldAlarm = false;

        #region Property : Name

        private string _name = string.Empty;
        public const string NamePropertyName = "Name";
        /// <summary>
        /// Gets or sets the "Name" property
        /// </summary>
        /// <value>The property value.</value>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, NamePropertyName); }
        }

        #endregion

        #region Property : SkinTone

        private int _skinTone = 0;
        public const string SkinTonePropertyName = "SkinTone";
        /// <summary>
        /// Gets or sets the "SkinTone" property
        /// </summary>
        /// <value>The property value.</value>
        public int SkinTone
        {
            get { return _skinTone; }
            set { SetProperty(ref _skinTone, value, SkinTonePropertyName); }
        }

        #endregion

        #region Property : SkinToneColor

        private Color _skinToneColor = Color.Transparent;
        public const string SkinToneColorPropertyName = "SkinToneColor";
        /// <summary>
        /// Gets or sets the "SkinTone" property
        /// </summary>
        /// <value>The property value.</value>
        public Color SkinToneColor
        {
            get { return _skinToneColor; }
            set { SetProperty(ref _skinToneColor, value, SkinToneColorPropertyName); }
        }

        #endregion

        #region Property : SkinToneRemarks

        private string _skinToneRemarks = string.Empty;
        public const string SkinToneRemarksPropertyName = "SkinToneRemarks";
        /// <summary>
        /// Gets or sets the "SkinToneRemarks" property
        /// </summary>
        /// <value>The property value.</value>
        public string SkinToneRemarks
        {
            get { return _skinToneRemarks; }
            set { SetProperty(ref _skinToneRemarks, value, SkinToneRemarksPropertyName); }
        }

        #endregion

        #region Property : SkinToneText

        private string _skinToneText = string.Empty;
        public const string SkinToneTextPropertyName = "SkinToneText";
        /// <summary>
        /// Gets or sets the "SkinToneText" property
        /// </summary>
        /// <value>The property value.</value>
        public string SkinToneText
        {
            get { return _skinToneText; }
            set { SetProperty(ref _skinToneText, value, SkinToneTextPropertyName); }
        }

        #endregion

        #region Property : SpfLevel

        private int _spfLevel = 0;
        public const string SpfLevelPropertyName = "SpfLevel";
        /// <summary>
        /// Gets or sets the "UserSpf" property
        /// </summary>
        /// <value>The property value.</value>
        public int SpfLevel
        {
            get { return _spfLevel; }
            set { SetProperty(ref _spfLevel, value, SpfLevelPropertyName); }
        }

        #endregion

        #region Property : SpfLevelTextColor

        private Color _spfLevelTextColor = Color.Transparent;
        public const string SpfLevelTextColorPropertyName = "SpfLevelTextColor";
        /// <summary>
        /// Gets or sets the "SpfLevelTextColor" property
        /// </summary>
        /// <value>The property value.</value>
        public Color SpfLevelTextColor
        {
            get { return _spfLevelTextColor; }
            set { SetProperty(ref _spfLevelTextColor, value, SpfLevelTextColorPropertyName); }
        }

        #endregion

        #region Property : UserProfileImage

        private string _userProfileImage = string.Empty;
        public const string UserProfileImagePropertyName = "UserProfileImage";
        /// <summary>
        /// Gets or sets the "UserProfileImage" property
        /// </summary>
        /// <value>The property value.</value>
        public string UserProfileImage
        {
            get { return _userProfileImage; }
            set { SetProperty(ref _userProfileImage, value, UserProfileImagePropertyName); }
        }

        #endregion

        #region Property : NameEdit

        private string _nameEdit = string.Empty;
        public const string NameEditPropertyName = "NameEdit";
        /// <summary>
        /// Gets or sets the "Name" property
        /// </summary>
        /// <value>The property value.</value>
        public string NameEdit
        {
            get { return _nameEdit; }
            set { SetProperty(ref _nameEdit, value, NameEditPropertyName); }
        }

        #endregion

        #region Property : SkinToneEdit

        private int _skinToneEdit = 0;
        public const string SkinToneEditPropertyName = "SkinToneEdit";
        /// <summary>
        /// Gets or sets the "SkinTone" property
        /// </summary>
        /// <value>The property value.</value>
        public int SkinToneEdit
        {
            get { return _skinToneEdit; }
            set { SetProperty(ref _skinToneEdit, value, SkinToneEditPropertyName); }
        }

        #endregion

        #region Property : SkinToneColorEdit

        private Color _skinToneColorEdit = Color.Transparent;
        public const string SkinToneColorEditPropertyName = "SkinToneColorEdit";
        /// <summary>
        /// Gets or sets the "SkinTone" property
        /// </summary>
        /// <value>The property value.</value>
        public Color SkinToneColorEdit
        {
            get { return _skinToneColorEdit; }
            set { SetProperty(ref _skinToneColorEdit, value, SkinToneColorEditPropertyName); }
        }

        #endregion
        
        #region Property : SpfLevelEdit

        private int _spfLevelEdit = 0;
        public const string SpfLevelEditPropertyName = "SpfLevelEdit";
        /// <summary>
        /// Gets or sets the "UserSpf" property
        /// </summary>
        /// <value>The property value.</value>
        public int SpfLevelEdit
        {
            get { return _spfLevelEdit; }
            set { SetProperty(ref _spfLevelEdit, value, SpfLevelEditPropertyName); }
        }

        #endregion
        
        #region Property : DisplayAlarm

        private string _displayAlarm = string.Empty;
        public const string DisplayAlarmPropertyName = "DisplayAlarm";
        /// <summary>
        /// Gets or sets the "DisplayAlarm" property
        /// </summary>
        /// <value>The property value.</value>
        public string DisplayAlarm
        {
            get { return _displayAlarm; }
            set { SetProperty(ref _displayAlarm, value, DisplayAlarmPropertyName); }
        }

        #endregion

        #region Property : IsAlarmActive

        private bool _isAlarmActive = false;
        public const string IsAlarmActivePropertyName = "IsAlarmActive";
        /// <summary>
        /// Gets or sets the "IsAlarmActive" property
        /// </summary>
        /// <value>The property value.</value>
        public bool IsAlarmActive
        {
            get { return _isAlarmActive; }
            set
            {
                if (_isAlarmActive == value)
                    return;

                if (value)
                {
                    Task.Run(async () => await NewAlarmActivation());
                }
                else
                {
                    Debug.WriteLine($"> Needs to disable alarm");
                    DisableActiveAlarm();
                }
                SetProperty(ref _isAlarmActive, value, IsAlarmActivePropertyName);
            }
        }

        private async Task NewAlarmActivation()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (!_oldAlarm)
                {
                    Debug.WriteLine($"> Needs to activate alarm");
                    int uv = 0;
                    if (_band.CurrentUV > 1)
                    {
                        uv = Uv.UV_ToPublic(_band.CurrentUV);
                    }
                    else
                    {
                        uv = _weather.UvLevel;
                    }
                    if (uv >= 1)
                    {
                        var ttsb = Uv.CalcTimeToBurnInSeconds(SkinTone,
                            SpfLevel, uv,
                            _weather.Altitude);
                        SetNewAlarm(await _service.AddAlarm(this.UserItem.GroupId, UserItem.UserId,
                            DateTime.UtcNow,
                            (int) ttsb, Name));
                        Debug.WriteLine(
                            $"> TTSB duration : {ttsb} ({DisplayAlarm}), Altitude={_weather.Altitude}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"> {e.GetType()} : {e.Message} ({e.Source})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public UserViewModel(User user, Alarm alarm, bool fullTimeFormat)
        {
            _service = ServiceLocator.Instance.Resolve<ICloudService>();
            _band = ServiceLocator.Instance.Resolve<IBandService>();
            _weather = ServiceLocator.Instance.Resolve<IWeatherService>();

            this._fullTimeFormat = fullTimeFormat;
            this.UserItem = user;
            this._alarm = alarm;
            
            Name = UserItem.Name;
            SkinTone = UserItem.SkinTone;
            SpfLevel = UserItem.SpfLevel;
            UserProfileImage = UserItem.ImageUrl;
            SkinToneColor = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(UserItem.SkinTone);
            SpfLevelTextColor = SunnyDay.Client.Core.Utils.DataConvert.Spf.Spf_ToTextColor(UserItem.SkinTone);

            Debug.WriteLine($"> New UserViewModel created! ({Name})");

            if (_alarm != null)
            {
                if (_alarm.EndTime.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds < 0)
                {
                    _service.DisableAlarm(_alarm.Id);
                    _alarm = null;
                    IsAlarmActive = false;
                    _oldAlarm = false;
                    DisplayAlarm = _fullTimeFormat ? "00:00:00" : "00:00";
                }
                else
                {
                    _oldAlarm = true;
                    IsAlarmActive = true;
                    UpdateTimer();
                }
            }
            else
            {
                IsAlarmActive = false;
                _oldAlarm = false;
                DisplayAlarm = _fullTimeFormat ? "00:00:00" : "00:00";
            }
        }

        public async Task UpdateUser()
        {
            UserItem.Name = NameEdit;
            UserItem.SkinTone = SkinToneEdit;
            UserItem.SpfLevel = SpfLevelEdit;

            await _service.UpdateUser(UserItem);
        }

        public void InitializeEditView()
        {
            NameEdit = UserItem.Name;
            SkinToneEdit = UserItem.SkinTone;
            SkinToneColorEdit = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(UserItem.SkinTone);
            SkinToneRemarks = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkiTone_ToRemarks(UserItem.SkinTone);
            SkinToneText = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToDescription(UserItem.SkinTone);
            SpfLevelEdit = UserItem.SpfLevel;

            Debug.WriteLine($"> EDIT: {NameEdit}, {SkinToneEdit}, {SpfLevelEdit}");
        }

        public void UpdateSkinToneEdit(int skinTone)
        {
            SkinToneEdit = skinTone;
            SkinToneColorEdit = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(skinTone);
            SkinToneRemarks = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkiTone_ToRemarks(skinTone);
            SkinToneText = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToDescription(skinTone);

            Debug.WriteLine($"> EDIT: {NameEdit}, {SkinToneEdit}, {SpfLevelEdit}");
        }

        public void UpdateTimer()
        {
            if (_alarm != null)
            {
                _timer = _alarm.EndTime.ToUniversalTime().Subtract(DateTime.UtcNow);
                DisplayAlarm = _fullTimeFormat ? _timer.ToString(@"hh\:mm\:ss") : _timer.ToString(@"hh\:mm");
            }
        }

        public void ZeroDisplayAlarm()
        {
            DisplayAlarm = _fullTimeFormat ? "00:00:00" : "00:00";
        }

        public void SetNewAlarm(Alarm alarm)
        {
            this._alarm = alarm;
            UpdateTimer();
            Debug.WriteLine($"> Timer Changed! ({Name}) {_timer.ToString("g")}");
        }

        public void DisableActiveAlarm()
        {
            if (_alarm != null) _service.DisableAlarm(_alarm.Id);
            this._alarm = null;
            this._timer = TimeSpan.Zero;
            ZeroDisplayAlarm();
            _oldAlarm = false;
        }
    }
}
