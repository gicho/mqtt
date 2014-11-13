﻿using System.Linq;

namespace Hermes
{
	public class TopicEvaluator : ITopicEvaluator
	{
		public bool IsValidTopicFilter (string topicFilter)
		{
			if (string.IsNullOrEmpty (topicFilter))
				return false;

			if (topicFilter.Length > 65536)
				return false;

			var topicFilterParts = topicFilter.Split ('/');

			if(topicFilterParts.Count(s => s == "#") > 1)
				return false;

			if (topicFilterParts.Any (s => s.Length > 1 && s.Contains ("#")))
				return false;

			if (topicFilterParts.Any (s => s.Length > 1 && s.Contains ("+")))
				return false;

			if(topicFilterParts.Any(s => s == "#") && topicFilter.IndexOf("#") < topicFilter.Length - 1)
				return false;

			return true;
		}

		public bool IsValidTopicName (string topicName)
		{
			return !string.IsNullOrEmpty (topicName) &&
				topicName.Length <= 65536 &&
				!topicName.Contains ("#") &&
				!topicName.Contains ("+");
		}

		public bool Matches (string topicName, string topicFilter)
		{
			var topicFilterParts = topicFilter.Split ('/');
			var topicNameParts = topicName.Split ('/');

			if (topicFilterParts.Length - topicNameParts.Length > 1)
				return false;

			if (topicFilterParts.Length - topicNameParts.Length == 1 && topicFilterParts[topicFilterParts.Length -1] != "#")
				return false;

			if ((topicFilterParts[0] == "#" || topicFilterParts[0] == "+") && topicNameParts[0].StartsWith ("$"))
				return false;

			var matches = true;

			for (var i = 0; i < topicFilterParts.Length; i++) {
				var topicFilterPart = topicFilterParts[i];

				if (topicFilterPart == "#") {
					matches = true;
					break;
				}

				if (topicFilterPart == "+") {
					if (i == topicFilterParts.Length - 1 && topicNameParts.Length > topicFilterParts.Length) {
						matches = false;
						break;
					}

					continue;
				}

				if (topicFilterPart != topicNameParts[i]) {
					matches = false;
					break;
				}
			}

			return matches;
		}
	}
}
