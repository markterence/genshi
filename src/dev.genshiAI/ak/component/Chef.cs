using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;

namespace dev.genshiAI.ak.component
{
	class Chef
	{
		public enum ChefState
		{

		}
		public List<Bitmap> recipe;
		public Rectangle chefTable;
        public float similarityOffset = 0.80f;
		public Chef()
		{
            recipe = new List<Bitmap>();
		}

		public void setRecipe(List<Bitmap> recipe)
		{
			this.recipe = recipe;
		}

		/// <summary>
		/// Setup camera
		/// </summary>
		/// <param name="rect">Rectangle from Region Select</param>
		public void setupChefTable(Rectangle rect)
		{
			this.chefTable = rect;
		}

		/// <summary>
		/// Performs Template Matching Techniques. Use at own risk.
		/// If out is -1, that means it doesn't match the tempalte.
		/// </summary>
		/// <param name="position">Screen position of detected image</param>
		/// <param name="size">Estimated size (width and height) of image.</param>
		/// <param name="estimatePos">estimated click position.</param>
		public void findWrongIngredient(Point position, Point estimateSize, out Point estimatePos)
		{
			//Do image process here
			//Get Image at Screen Position of 'position' and set size
			//Check if Image at that `screen position` matches with the template.
			int x = -1;
			int y = -1;
			x = position.X / ((estimateSize.X == 0) ? 1 : estimateSize.X);
			y = position.Y / ((estimateSize.Y == 0) ? 1 : estimateSize.Y);
			estimatePos = new Point(x, y);
		}

		BlobCounter blobCounter;
		public float treshold = 0.79f;
		int yOffset = 64, xOffset = 64;

        /// <summary>
        /// Performs EuclidColorFilter, BlobCouter and Template Matching
        /// Input: image 
        /// </summary>
        /// <param name="sourceImage">From Region Selection</param>
        /// <param name="blobedImage"></param>
        /// <param name="findingsImage"></param>
        public void chefEuclidFilter(Bitmap sourceImage, out List<string> similarity, out Point click, out Bitmap blobedImage, out Bitmap findingsImage)
        {
            Point cp = new Point();
            similarity = new List<string>();
            Bitmap sourceClone = sourceImage.Clone(
                new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                sourceImage.PixelFormat);
            EuclideanColorFiltering euFilter = new EuclideanColorFiltering();
			Bitmap filteredBitmap = sourceImage.Clone(
				new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), 
				sourceImage.PixelFormat);

			euFilter.CenterColor = new AForge.Imaging.RGB(Color.FromArgb(211, 203, 202, 200));
			euFilter.Radius = 25;
			euFilter.ApplyInPlace(filteredBitmap);

			int blobCtr = 0;
			unsafe
			{
				BitmapData bitData = filteredBitmap.LockBits(
					new Rectangle(0, 0, filteredBitmap.Width, filteredBitmap.Height),
					ImageLockMode.ReadOnly, filteredBitmap.PixelFormat);

				blobCounter = new BlobCounter();
				blobCounter.FilterBlobs = true;
				blobCounter.ProcessImage(bitData);

				Blob[] blobs = blobCounter.GetObjectsInformation();

				ExhaustiveTemplateMatching tempalteMatching = new ExhaustiveTemplateMatching(treshold);

				//Loop on Images
				int errorOffset = 3;
				foreach (var item in recipe)
				{
					foreach (var blob in blobs)
					{
						//Detected too small
						if (blob.Rectangle.Width <= errorOffset) continue;
						//out of screen
						if (((blob.Rectangle.X) - errorOffset) < 0) continue;
						int estimateY = (blob.Rectangle.Y + yOffset);
						int estimateX = (blob.Rectangle.X + xOffset);

						if (!(estimateY >= sourceImage.Size.Height) && !(estimateX >= sourceImage.Size.Width))
						{
							//Hightlight blobs
							Drawing.Rectangle(bitData, blob.Rectangle, Color.Red);
							blobCtr++;
							
							//Finder Block Size
							Rectangle rectSize = new Rectangle(
								blob.Rectangle.X - errorOffset, blob.Rectangle.Y - errorOffset,
								64, 64);

							TemplateMatch[] matchings = tempalteMatching.ProcessImage(sourceClone, item, rectSize);
							unsafe
							{
								BitmapData data = sourceClone.LockBits(
									new Rectangle(0,0, sourceClone.Width, sourceClone.Height),
									ImageLockMode.ReadOnly, sourceClone.PixelFormat);

								foreach (var match in matchings)
								{
                                    
                                    //Highlight Matches
                                    if (match.Similarity >= this.similarityOffset && match.Similarity < 0.90f)
                                    {
                                        Drawing.Rectangle(data, match.Rectangle, Color.Fuchsia);
                                        similarity.Add(match.Similarity.ToString());
                                        int clickX = this.chefTable.X + match.Rectangle.X-1;
                                        int clickY = this.chefTable.Y + match.Rectangle.Y-1;
                                        cp = new Point(clickX, clickY);
                                        click = cp;
                                    }
								}
                                sourceClone.UnlockBits(data);
							}//end of unsafe teamplate maching
						}//end of if condition
                    }//end of blob loop
				}//end of ingredient image loop
                filteredBitmap.UnlockBits(bitData);
            }//end of unsafe blob filter
            if (similarity.Count <= 0)
                cp = new Point(-1, -1);
            blobedImage = filteredBitmap;
            findingsImage = sourceClone;
            click = cp;
		}
	}
}
