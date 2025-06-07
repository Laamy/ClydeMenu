using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Overtone.Scripts
{
	// Token: 0x020002D3 RID: 723
	public static class SSMLPreprocessor
	{
		// Token: 0x0600168E RID: 5774 RVA: 0x000C6968 File Offset: 0x000C4B68
		public static SpeechUnit[] Preprocess(string ssml)
		{
			if (ssml.Length == 0)
			{
				return Array.Empty<SpeechUnit>();
			}
			List<SpeechUnit> list = new List<SpeechUnit>();
			StringBuilder stringBuilder = new StringBuilder();
			using (StringReader stringReader = new StringReader(ssml))
			{
				while (stringReader.Peek() != -1)
				{
					char c = (char)stringReader.Read();
					if (c == '<')
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						while (stringReader.Peek() != -1 && (c = (char)stringReader.Read()) != '>')
						{
							stringBuilder2.Append(c);
						}
						string text = stringBuilder2.ToString();
						if (text.StartsWith("break"))
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine();
						}
						else if (text.StartsWith("prosody"))
						{
							list.Add(new SpeechUnit
							{
								Text = stringBuilder.ToString()
							});
							stringBuilder.Clear();
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				list.Add(new SpeechUnit
				{
					Text = stringBuilder.ToString()
				});
			}
			return list.ToArray();
		}
	}
}
