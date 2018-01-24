using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Safire.Fx.FxModels
{
	[Serializable]
	public class Suite
	{
		public Booster Booster { get; set; }
		public List<float> EqValuesList { get; set; }
		public float BassEn { get; set; }
		public float StereoEn { get; set; }

		public bool BoosterStatus { get; set; }
		public bool EqStatus { get; set; }
		public bool BassEnStatus { get; set; }
		public bool StereoEnStatus { get; set; }

	}
}
