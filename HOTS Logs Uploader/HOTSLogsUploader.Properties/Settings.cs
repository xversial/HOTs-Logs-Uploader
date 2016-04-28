using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HOTSLogsUploader.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string ReplayDirectory
		{
			get
			{
				return (string)this["ReplayDirectory"];
			}
			set
			{
				this["ReplayDirectory"] = value;
			}
		}

		[DefaultSettingValue(":"), UserScopedSetting, DebuggerNonUserCode]
		public string ReplaysUploaded
		{
			get
			{
				return (string)this["ReplaysUploaded"];
			}
			set
			{
				this["ReplaysUploaded"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool ShowTrayNotifications
		{
			get
			{
				return (bool)this["ShowTrayNotifications"];
			}
			set
			{
				this["ShowTrayNotifications"] = value;
			}
		}

		[DefaultSettingValue("-1"), UserScopedSetting, DebuggerNonUserCode]
		public int PlayerID
		{
			get
			{
				return (int)this["PlayerID"];
			}
			set
			{
				this["PlayerID"] = value;
			}
		}
	}
}
