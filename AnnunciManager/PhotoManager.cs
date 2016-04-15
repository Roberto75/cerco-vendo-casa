using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annunci
{
    public class PhotoManager
        : MyManagerCSharp.ManagerDB
    {

        public PhotoManager(string connectionName)
            : base(connectionName)
        {

        }

        public PhotoManager(System.Data.Common.DbConnection connection)
            : base(connection)
        {

        }


        public bool deletePhoto(long photoId, string absoluteServerPath)
        {
            string path;
            mStrSQL = "SELECT PATH FROM PHOTO WHERE PHOTO_ID = " + photoId;
            path = mExecuteScalar(mStrSQL);

            if (!String.IsNullOrEmpty(path) && !path.StartsWith("http") && !String.IsNullOrEmpty(absoluteServerPath))
            {

                if (path.StartsWith("~"))
                {
                    path = path.Substring(1);
                }


                //'cancellazione su file system della photo originale
                System.IO.FileInfo f = new System.IO.FileInfo(absoluteServerPath + path);
                if (f.Exists)
                {
                    f.Delete();
                }


                //'cancellazione su file system della Thumbnail
                string temp;
                temp = f.DirectoryName + "/thumbnail_" + f.Name;
                System.IO.FileInfo t = new System.IO.FileInfo(temp);

                if (t.Exists)
                {
                    t.Delete();
                }


            }

            mStrSQL = "DELETE * FROM PHOTO WHERE PHOTO_ID = " + photoId;
            return mExecuteNoQuery(mStrSQL) > 0;
        }



        public long insertPhoto(string absolutePath, string description, long externalId, long userId)
        {

            mStrSQL = "INSERT INTO PHOTO ( PATH, DESCRIPTION, FK_USER_ID, FK_EXTERNAL_ID) " +
                       " VALUES ( @PATH , @DESCRIPTION , @FK_USER_ID ,  @FK_EXTERNAL_ID   ) ";

            System.Data.Common.DbCommand command;
            command = mConnection.CreateCommand();

            mAddParameter(command, "@PATH", absolutePath);
            mAddParameter(command, "@DESCRIPTION", description);
            mAddParameter(command, "@FK_USER_ID", userId);
            mAddParameter(command, "@FK_EXTERNAL_ID", externalId);

            command.CommandText = mStrSQL;

            mExecuteNoQuery(command);

            return mGetIdentity();

        }


        public System.Data.DataTable getPhotosIsPlanimetria(long externalId)
        {
            mStrSQL = "SELECT * FROM PHOTO WHERE  DATE_DELETED IS NULL AND DESCRIPTION = 'PLANIMETRIA' AND  FK_EXTERNAL_ID  = " + externalId;
            return mFillDataTable(mStrSQL);
        }

        public System.Data.DataTable getPhotosIsNotPlanimetria(long externalId)
        {
            mStrSQL = "SELECT * FROM PHOTO WHERE  DATE_DELETED IS NULL AND DESCRIPTION <> 'PLANIMETRIA' AND  FK_EXTERNAL_ID  = " + externalId;
            return mFillDataTable(mStrSQL);
        }



        public List<Models.MyPhoto> getMyPhotosIsNotPlanimetria(long externalId)
        {

            mDt = getPhotosIsNotPlanimetria(externalId);

            List<Models.MyPhoto> result = new List<Models.MyPhoto>();
            Models.MyPhoto photo;

            foreach (System.Data.DataRow row in mDt.Rows)
            {
                photo = new Models.MyPhoto();

                photo.PhotoId = long.Parse(row["photo_id"].ToString());
                photo.Path = row["path"].ToString();
                photo.Note = row["description"].ToString();

                result.Add(photo);
            }

            return result;
        }


        public static System.Drawing.Image resizeImageWidth(System.Drawing.Image imgToResize, int newSizeWidth)
        {
            return Annunci.PhotoManager.resizeImage(imgToResize, newSizeWidth, -1);
        }

        public static System.Drawing.Image resizeImageHeight(System.Drawing.Image imgToResize, int newSizeHeight)
        {
            return Annunci.PhotoManager.resizeImage(imgToResize, -1, newSizeHeight);
        }


        public static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, int newSizeWidth, int newSizeHeight)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;


            double nPercentW = 0;
            double nPercentH = 0;

            nPercentW = (double)newSizeWidth / (double)sourceWidth;
            nPercentH = (double)newSizeHeight / (double)sourceHeight;

            if (newSizeWidth == -1)
            {
                nPercentW = nPercentH;
            }

            if (newSizeHeight == -1)
            {
                nPercentH = nPercentW;
            }



            int destWidth = (int)(sourceWidth * nPercentW);
            int destHeight = (int)(sourceHeight * nPercentH);

            System.Drawing.Bitmap b = new System.Drawing.Bitmap(destWidth, destHeight);

            System.Drawing.Graphics g;

            g = System.Drawing.Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }



        public static System.Drawing.Image getThumbnailImage(string absolutePathImage, int width, int height)
        {
            System.Drawing.Image oldImage;
            oldImage = System.Drawing.Image.FromFile(absolutePathImage);

            return getThumbnailImage(oldImage, width, height);
        }


        public static System.Drawing.Image getThumbnailImage(System.Drawing.Image oldImage, int width, int height)
        {
            return oldImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }


    }
}
