using System;

namespace HOTSLogsUploader
{
	internal class ReplayFile
	{
		public string FileName;

		public string UploadStatus;

		public DateTime? DateTimeUploaded;

		public ReplayFile(string fileName, string uploadStatus, DateTime? dateTimeUploaded)
		{
			this.FileName = fileName;
			this.UploadStatus = uploadStatus;
			this.DateTimeUploaded = dateTimeUploaded;
		}
	}
}
