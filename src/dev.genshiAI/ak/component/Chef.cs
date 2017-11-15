using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dev.genshiAI.ak.component
{
	class Chef
	{
		public enum ChefState
		{

		}
		public List<Bitmap> recipe;
		public Rectangle chefTable;

		public Chef()
		{
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
	}
}
