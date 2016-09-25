using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Weathi.Model;

namespace Weathi
{
	public class ForecastItemAdapter : BaseAdapter<Forecast>
	{
		List<Forecast> items;
		Activity context;

		public ForecastItemAdapter (Activity context, List<Forecast> items): base()
		{
			this.context = context;
			this.items = items;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Forecast this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.ForecastItem, null);
			
			view.FindViewById<TextView>(Resource.Id.txtDay).Text = item.Day;
			view.FindViewById<TextView>(Resource.Id.txtHigh).Text = item.High;
			view.FindViewById<TextView>(Resource.Id.txtLow).Text = item.Low;
			view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ResourceID);

			return view;
		}
	}
}

