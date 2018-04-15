using System;
using System.Net;

namespace WorkingTools.Classes.FtpProviderPart
{
    public enum FtpRequestType
    {
        /// <summary>
        /// ������������ ����� ��������� FTP APPE, ������� ������������ ��� ������������� ����� � ������������� ����� �� FTP-�������.
        /// </summary>
        DownloadFile,
        /// <summary>
        /// ������������ ����� ��������� FTP DELE, ������� ������������ ��� �������� ����� �� FTP-�������.
        /// </summary>
        ListDirectory,
        /// <summary>
        /// ������������ ����� ��������� FTP RETR, ������� ������������ ��� �������� ����� � FTP-�������.
        /// </summary>
        UploadFile,
        /// <summary>
        /// ������������ ����� ��������� FTP MDTM, ������� ������������ ��� ��������� ������ ���� � ������� �� ����� �� FTP-�������.
        /// </summary>
        DeleteFile,
        /// <summary>
        /// ������������ ����� ��������� FTP SIZE, ������� ������������ ��� ��������� ������� ����� �� FTP-�������.
        /// </summary>
        AppendFile,
        /// <summary>
        /// ������������ ����� ��������� FTP NLIST, ������� ���������� ������� ������ ������ �� FTP-�������.
        /// </summary>
        GetFileSize,
        /// <summary>
        /// ������������ ����� ��������� FTP LIST, ������� ���������� ��������� ������ ������ �� FTP-�������.
        /// </summary>
        UploadFileWithUniqueName,
        /// <summary>
        /// ������������ ����� ��������� FTP MKD, ������� ������� ������� �� FTP-�������.
        /// </summary>
        MakeDirectory,
        /// <summary>
        /// ������������ ����� ��������� FTP PWD, ������� ���������� ��� �������� �������� ��������.
        /// </summary>
        RemoveDirectory,
        /// <summary>
        /// ������������ ����� ��������� FTP RMD, ������� ������� �������.
        /// </summary>
        ListDirectoryDetails,
        /// <summary>
        /// ������������ ����� ��������� FTP RENAME, ������� ��������������� �������.
        /// </summary>
        GetDateTimestamp,
        /// <summary>
        /// ������������ ����� ��������� FTP STOR, ������� ��������� ���� �� FTP-������.
        /// </summary>
        PrintWorkingDirectory,
        /// <summary>
        /// ������������ ����� ��������� FTP STOU, ������� ��������� ���� � ���������� ������ �� FTP-������.
        /// </summary>
        Rename,
    }

    public static class FtpRequestTypeExtention
    {
        public static string ToFtpWebRequestMethod(this FtpRequestType ftpRequestType)
        {
            switch (ftpRequestType)
            {
                case FtpRequestType.DownloadFile:
                    return WebRequestMethods.Ftp.DownloadFile;
                case FtpRequestType.ListDirectory:
                    return WebRequestMethods.Ftp.ListDirectory;
                case FtpRequestType.UploadFile:
                    return WebRequestMethods.Ftp.UploadFile;
                case FtpRequestType.DeleteFile:
                    return WebRequestMethods.Ftp.DeleteFile;
                case FtpRequestType.AppendFile:
                    return WebRequestMethods.Ftp.AppendFile;
                case FtpRequestType.GetFileSize:
                    return WebRequestMethods.Ftp.GetFileSize;
                case FtpRequestType.UploadFileWithUniqueName:
                    return WebRequestMethods.Ftp.UploadFileWithUniqueName;
                case FtpRequestType.MakeDirectory:
                    return WebRequestMethods.Ftp.MakeDirectory;
                case FtpRequestType.RemoveDirectory:
                    return WebRequestMethods.Ftp.RemoveDirectory;
                case FtpRequestType.ListDirectoryDetails:
                    return WebRequestMethods.Ftp.ListDirectoryDetails;
                case FtpRequestType.GetDateTimestamp:
                    return WebRequestMethods.Ftp.GetDateTimestamp;
                case FtpRequestType.PrintWorkingDirectory:
                    return WebRequestMethods.Ftp.PrintWorkingDirectory;
                case FtpRequestType.Rename:
                    return WebRequestMethods.Ftp.Rename;
                default:
                    throw new ArgumentOutOfRangeException("ftpRequestType", "�������� �� �������������");
            }
        }
    }
}