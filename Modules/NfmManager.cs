using DataCore;
using MapEditor.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MapEditor.Modules
{
	public class NfmManager : Nfm
	{
		public int[] SupportedVersion = { 16, 17, 19, 21, 22 };

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public NfmManager() : base() { Blank(); }

		/// <summary>
		/// Reinitilize child objet
		/// </summary>
		public void Blank()
		{
			TileCountPerSegment = 6;
			SegmentCountPerMap = 64;
			TileLenght = 42;
			MapProperties = new MapProperties();
			DwTerrainSegment = new KSegment[64, 64];

			for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
				for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
				{
					DwTerrainSegment[segmentX, segmentY] = new KSegment();
					for (int titleY = 0; titleY < TileCountPerSegment; titleY++)
						for (int titleX = 0; titleX < TileCountPerSegment; titleX++)
						{
							DwTerrainSegment[segmentX, segmentY].HsVector[titleX, titleY] = new KVertex();
						}
				}

			DwProps.Clear();
			DwGrass.Clear();
			DwVectorAttr.Clear();
			DwWater.Clear();
			DwGrassColony.Clear();
			DwEventArea.Clear();
		}

		/// <summary>
		/// get buffer final file
		/// </summary>
		/// <returns></returns>
		public byte[] GetBuffer()
		{
			try
			{
				var stream = new MemoryStream();

				using (BinaryWriter b = new BinaryWriter(stream))
				{
					b.Write(Encoding.Default.GetBytes(Sign));
					b.Write(22);

					b.Write(0); /* dwMapPropertiesOffset = */
					b.Write(0); /* dwTerrainSegmentOffset = */
					b.Write(0); /* dwPropOffset = */
					b.Write(0); /* dwVectorAttrOffset = */
					b.Write(0); /* dwWaterOffset = */
					b.Write(0); /* dwGrassColonyOffset = */
					b.Write(0); /* dwEventAreaOffset = */

					b.Write(TileCountPerSegment);
					b.Write(SegmentCountPerMap);
					b.Write(TileLenght);

					#region Properties

					int dwMapPropertiesOffset = (int)b.BaseStream.Position;

					b.Write(MapProperties.Primary.Diffuse.Color.B);
					b.Write(MapProperties.Primary.Diffuse.Color.G);
					b.Write(MapProperties.Primary.Diffuse.Color.R);
					b.Write(MapProperties.Primary.Specular.Color.B);
					b.Write(MapProperties.Primary.Specular.Color.G);
					b.Write(MapProperties.Primary.Specular.Color.R);
					b.Write(MapProperties.Primary.Attenuation0);
					b.Write(MapProperties.Primary.Attenuation1);
					b.Write(MapProperties.Primary.Attenuation2);
					b.Write(MapProperties.Secondary.Diffuse.Color.B);
					b.Write(MapProperties.Secondary.Diffuse.Color.G);
					b.Write(MapProperties.Secondary.Diffuse.Color.R);
					b.Write(MapProperties.Secondary.Specular.Color.B);
					b.Write(MapProperties.Secondary.Specular.Color.G);
					b.Write(MapProperties.Secondary.Specular.Color.R);
					b.Write(MapProperties.Secondary.Attenuation0);
					b.Write(MapProperties.Secondary.Attenuation1);
					b.Write(MapProperties.Secondary.Attenuation2);
					b.Write(MapProperties.Sky.Color.B);
					b.Write(MapProperties.Sky.Color.G);
					b.Write(MapProperties.Sky.Color.R);
					b.Write(MapProperties.Fog.Color.B);
					b.Write(MapProperties.Fog.Color.G);
					b.Write(MapProperties.Fog.Color.R);
					b.Write(MapProperties.FogNear);
					b.Write(MapProperties.FogFar);
					b.Write(MapProperties.SkyType);
					b.Write(MapProperties.ShowTerrainInGame);

					#endregion

					#region Terrain segment

					int dwTerrainSegmentOffset = (int)b.BaseStream.Position;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							b.Write(DwTerrainSegment[segmentX, segmentY].Version);

							for (int tile = 0; tile < 3; tile++)
							{
								b.Write(DwTerrainSegment[segmentX, segmentY].Tile[tile]);
							}

							for (int titleY = 0; titleY < TileCountPerSegment; titleY++)
								for (int tileX = 0; tileX < TileCountPerSegment; tileX++)
								{
									b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Height);

									for (int f = 0; f < 2; f++)
									{
										b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].FillBits[f]);
									}

									b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Attribute);
									b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.B);
									b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.G);
									b.Write(DwTerrainSegment[segmentX, segmentY].HsVector[tileX, titleY].Color.Color.R);
								}
						}

					#endregion

					#region Prop

					int dwPropOffset = (int)b.BaseStream.Position;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							b.Write(0);
						}

					int index = 0;
					int segment = 0;

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							var offset = (int)b.BaseStream.Position;
							b.Seek(dwPropOffset + (4 * segment), SeekOrigin.Begin);
							b.Write(offset);
							b.Seek(offset, SeekOrigin.Begin);

							var prop = DwProps.Where(r => r.SegmentId == segment).ToList();

							b.Write(prop.Count);

							for (int p = 0; p < prop.Count; p++)
							{
								b.Write(index);
								b.Write(prop[p].X);
								b.Write(prop[p].Y);
								b.Write(prop[p].Z);
								b.Write(prop[p].RotateX);
								b.Write(prop[p].RotateY);
								b.Write(prop[p].RotateZ);
								b.Write(prop[p].ScaleX);
								b.Write(prop[p].ScaleY);
								b.Write(prop[p].ScaleZ);
								b.Write(prop[p].PropNum);
								b.Write(prop[p].HeightLocked);
								b.Write(prop[p].LockHeight);
								b.Write(prop[p].TextureGroup);
								index++;
							}

							var grass = DwGrass.Where(r => r.SegmentId == segment).ToList();

							b.Write(grass.Count);

							for (int n = 0; n < grass.Count; n++)
							{
								b.Write(grass[n].GrassId);
								b.Write(grass[n].Props.Count);

								for (int i = 0; i < grass[n].Props.Count; i++)
								{
									b.Write(grass[n].Props[i].X);
									b.Write(grass[n].Props[i].Y);
									b.Write(grass[n].Props[i].RotateX);
									b.Write(grass[n].Props[i].RotateY);
									b.Write(grass[n].Props[i].RotateZ);
								}
							}

							segment++;
						}

					#endregion

					#region Vector attribute

					int dwVectorAttrOffset = (int)b.BaseStream.Position;

					b.Write(Nfa.Instance.Polygons.Count);

					for (int i = 0; i < Nfa.Instance.Polygons.Count; i++)
					{
						b.Write(Nfa.Instance.Polygons[i].Points.Count);

						for (int p = 0; p < Nfa.Instance.Polygons[i].Points.Count; p++)
						{
							b.Write(Nfa.Instance.Polygons[i].Points[p].X);
							b.Write(Nfa.Instance.Polygons[i].Points[p].Y);
						}
					}

					#endregion

					#region Water

					int dwWaterOffset = (int)b.BaseStream.Position;

					b.Write(DwWater.Count);

					for (int i = 0; i < DwWater.Count; i++)
					{
						b.Write(DwWater[i].PointA.X);
						b.Write(DwWater[i].PointA.Y);
						b.Write(DwWater[i].PointA.Z);
						b.Write(DwWater[i].PointB.X);
						b.Write(DwWater[i].PointB.Y);
						b.Write(DwWater[i].PointB.Z);
						b.Write(DwWater[i].Center.X);
						b.Write(DwWater[i].Center.Y);
						b.Write(DwWater[i].Center.Z);
						b.Write(DwWater[i].UseReflect);
						b.Write(DwWater[i].WaterId);
					}

					#endregion

					#region Speed grass

					int dwGrassColonyOffset = (int)b.BaseStream.Position;

					b.Write(DwGrassColony.Count);

					for (int i = 0; i < DwGrassColony.Count; i++)
					{
						b.Write(i + 1);
						b.Write(DwGrassColony[i].Density);
						b.Write(DwGrassColony[i].Distribution);
						b.Write(DwGrassColony[i].Size);
						b.Write(DwGrassColony[i].HeightP);
						b.Write(DwGrassColony[i].HeightM);
						b.Write(DwGrassColony[i].Color.Color.B);
						b.Write(DwGrassColony[i].Color.Color.G);
						b.Write(DwGrassColony[i].Color.Color.R);
						b.Write(DwGrassColony[i].Color.Color.A);
						b.Write(DwGrassColony[i].ColorRatio);
						b.Write(DwGrassColony[i].ColorTone);
						b.Write(DwGrassColony[i].Chroma);
						b.Write(DwGrassColony[i].Brightness);
						b.Write(DwGrassColony[i].CombinationRatio);
						b.Write(DwGrassColony[i].WindReaction);

						var texture = DwGrassColony[i].Filename.Length == 0 ?
							DwGrassColony[i].Filename :
							DwGrassColony[i].Filename.Replace("\0", "") + '\0';

						b.Write(texture.Length);
						b.Write(Encoding.Default.GetBytes(texture));
						b.Write(DwGrassColony[i].Polygons.Count);

						for (int p = 0; p < DwGrassColony[i].Polygons.Count; p++)
						{
							b.Write(DwGrassColony[i].Polygons[p].Points.Count);

							for (int n = 0; n < DwGrassColony[i].Polygons[p].Points.Count; n++)
							{
								b.Write(DwGrassColony[i].Polygons[p].Points[n].X);
								b.Write(DwGrassColony[i].Polygons[p].Points[n].Y);
							}
						}
					}

					#endregion

					#region Event area

					int dwEventAreaOffset = (int)b.BaseStream.Position;

					b.Write(Nfe.Instance.Events.Count);

					for (int i = 0; i < Nfe.Instance.Events.Count; i++)
					{
						b.Write(Nfe.Instance.Events[i].AreaId);
						b.Write(Nfe.Instance.Events[i].Polygons.Count);

						for (int p = 0; p < Nfe.Instance.Events[i].Polygons.Count; p++)
						{
							b.Write(Nfe.Instance.Events[i].Polygons[p].Points.Count);

							for (int n = 0; n < Nfe.Instance.Events[i].Polygons[p].Points.Count; n++)
							{
								b.Write(Nfe.Instance.Events[i].Polygons[p].Points[n].X);
								b.Write(Nfe.Instance.Events[i].Polygons[p].Points[n].Y);
							}
						}
					}

					#endregion

					b.Seek(20, SeekOrigin.Begin);
					b.Write(dwMapPropertiesOffset);
					b.Write(dwTerrainSegmentOffset);
					b.Write(dwPropOffset);
					b.Write(dwVectorAttrOffset);
					b.Write(dwWaterOffset);
					b.Write(dwGrassColonyOffset);
					b.Write(dwEventAreaOffset);
				}

				XLog.WriteLine(Levels.Good, "Ok");
				return stream.ToArray();
			}
			catch (Exception exception)
			{
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfmManager::GetBuffer<Exception> -> {0}", exception);
			}

			return null;
		}

		/// <summary>
		/// Load existing map
		/// </summary>
		/// <param name="buffer"></param>
		public void Load(byte[] buffer)
		{
			try
			{
				using (BinaryReader b = new BinaryReader(new MemoryStream(buffer)))
				{
					Sign = Encoding.Default.GetString(b.ReadBytes(16));
					Version = b.ReadInt32();

#if DEBUG == false
					if (!SupportedVersion.Contains(Version))
					{
						XLog.WriteLine(Levels.Error, $"Failed");
						XLog.WriteLine(Levels.Error, "Incompatible version {0} is not supported or not implemented.", Version);
						return;
					}
#endif

					var dwMapPropertiesOffset = b.ReadInt32();
					var dwTerrainSegmentOffset = b.ReadInt32();
					var dwPropOffset = b.ReadInt32();
					var dwVectorAttrOffset = b.ReadInt32();
					var dwWaterOffset = b.ReadInt32();
					var dwGrassColonyOffset = (Version >= 17) ? b.ReadInt32() : 0;
					var dwEventAreaOffset = (Version >= 22) ? b.ReadInt32() : 0;

					TileCountPerSegment = b.ReadInt32();
					SegmentCountPerMap = b.ReadInt32();
					TileLenght = b.ReadSingle();

	#region Properties
                    
					MapProperties.Primary.Diffuse = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.Primary.Specular = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.Primary.Attenuation0 = b.ReadSingle();
					MapProperties.Primary.Attenuation1 = b.ReadSingle();
					MapProperties.Primary.Attenuation2 = b.ReadSingle();
					MapProperties.Secondary.Diffuse = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.Secondary.Specular = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.Secondary.Attenuation0 = b.ReadSingle();
					MapProperties.Secondary.Attenuation1 = b.ReadSingle();
					MapProperties.Secondary.Attenuation2 = b.ReadSingle();
					MapProperties.Sky = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.Fog = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), 255);
					MapProperties.FogNear = b.ReadSingle();
					MapProperties.FogFar = b.ReadSingle();
					MapProperties.SkyType = b.ReadUInt32();
					MapProperties.ShowTerrainInGame = b.ReadBoolean();

	#endregion

	#region Terrain segment

					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							DwTerrainSegment[segmentX, segmentY] = new KSegment();
							DwTerrainSegment[segmentX, segmentY].Version = (Version >= 16) ? b.ReadUInt32() : 0;

							for (int tile = 0; tile < 3; tile++)
							{
								DwTerrainSegment[segmentX, segmentY].Tile[tile] = (Version >= 16) ? b.ReadUInt16() : (ushort)0;
							}

							DwTerrainSegment[segmentX, segmentY].HsVector = new KVertex[TileCountPerSegment, TileCountPerSegment];

							for (int tileY = 0; tileY < TileCountPerSegment; tileY++)
								for (int tileX = 0; tileX < TileCountPerSegment; tileX++)
								{
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY] = new KVertex();
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Height = b.ReadSingle();

									for (int f = 0; f < 2; f++)
									{
										if (Version >= 16) DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = b.ReadUInt32();
										if (Version == 15 && f == 0) DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].FillBits[f] = b.ReadUInt32();
									}

									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Attribute = (Version >= 16) ? b.ReadInt64() : 0;
									DwTerrainSegment[segmentX, segmentY].HsVector[tileX, tileY].Color = new KTripleColor(b.ReadByte(), b.ReadByte(), b.ReadByte());
								}
						}

	#endregion

	#region Prop
                    
					//Escape offset prop table
					for (int i = 0; i < SegmentCountPerMap * SegmentCountPerMap; i++)
					{
						b.ReadInt32();
					}

					// PROP
					var segment = 0;
					for (int segmentY = 0; segmentY < SegmentCountPerMap; segmentY++)
						for (int segmentX = 0; segmentX < SegmentCountPerMap; segmentX++)
						{
							var propcount = b.ReadInt32();

							for (int p = 0; p < propcount; p++)
							{
								var prop = new KProp();
								/* index */ b.ReadInt32();
								prop.SegmentId = segment;
								prop.X = b.ReadSingle();
								prop.Y = b.ReadSingle();
								prop.Z = b.ReadSingle();
								prop.RotateX = b.ReadSingle();
								prop.RotateY = b.ReadSingle();
								prop.RotateZ = b.ReadSingle();
								prop.ScaleX = b.ReadSingle();
								prop.ScaleY = (Version >= 21) ? b.ReadSingle() : prop.ScaleX;
								prop.ScaleZ = (Version >= 21) ? b.ReadSingle() : prop.ScaleX;
								prop.PropNum = b.ReadUInt16();
								prop.HeightLocked = (Version >= 21) ? b.ReadBoolean() : false;
								prop.LockHeight = (Version >= 21) ? b.ReadSingle() : 0.0f;
								prop.TextureGroup = (Version >= 21) ? b.ReadInt16() : (short)-1;
								DwProps.Add(prop);
							}

							if (Version >= 19)
							{
								var grassCount = b.ReadInt32();

								for (int n = 0; n < grassCount; n++)
								{
									var grass = new KGrass();
									grass.SegmentId = segment;
									grass.GrassId = b.ReadInt32();
									var propCount = b.ReadInt32();

									for (int i = 0; i < propCount; i++)
									{
										var prop = new KGrassProp();
										prop.X = b.ReadSingle();
										prop.Y = b.ReadSingle();
										prop.RotateX = b.ReadSingle();
										prop.RotateY = b.ReadSingle();
										prop.RotateZ = b.ReadSingle();
										grass.Props.Add(prop);
									}

									DwGrass.Add(grass);
								}
							}

							segment++;
						}

	#endregion

	#region Vector attribute
                    
					var polygonCount = b.ReadInt32();

					for (int i = 0; i < polygonCount; i++)
					{
						var polygon = new Polygon2();
						var pointNum = b.ReadInt32();

						for (int p = 0; p < pointNum; p++)
						{
							var point = new K2DPosition();
							point.X = b.ReadInt32();
							point.Y = b.ReadInt32();
							polygon.Points.Add(point);
						}

						DwVectorAttr.Add(polygon);
					}

	#endregion

	#region Water

					var waterCount = b.ReadInt32();
                    
					for (int i = 0; i < waterCount; i++)
					{
						var water = new Water();
						water.PointA.X = b.ReadSingle();
						water.PointA.Y = b.ReadSingle();
						water.PointA.Z = b.ReadSingle();
						water.PointB.X = b.ReadSingle();
						water.PointB.Y = b.ReadSingle();
						water.PointB.Z = b.ReadSingle();
						water.Center.X = b.ReadSingle();
						water.Center.Y = b.ReadSingle();
						water.Center.Z = b.ReadSingle();
						water.UseReflect = b.ReadInt32();
						water.WaterId = b.ReadInt32();
						DwWater.Add(water);
					}

					#endregion
                    
					if (Version >= 17)
					{
	#region Speed grass

						var speedGrassCount = b.ReadInt32();

						for (int i = 0; i < speedGrassCount; i++)
						{
							var speedGrass = new SpeedGrassColony();
							speedGrass.GrassId = b.ReadInt32();
							speedGrass.Density = b.ReadSingle();
							speedGrass.Distribution = b.ReadSingle();
							speedGrass.Size = b.ReadSingle();
							speedGrass.HeightP = b.ReadSingle();
							speedGrass.HeightM = b.ReadSingle();
							speedGrass.Color = new KColor(b.ReadByte(), b.ReadByte(), b.ReadByte(), b.ReadByte());
							speedGrass.ColorRatio = b.ReadSingle();
							speedGrass.ColorTone = b.ReadSingle();
							speedGrass.Chroma = b.ReadSingle();
							speedGrass.Brightness = b.ReadSingle();
							speedGrass.CombinationRatio = b.ReadSingle();
							speedGrass.WindReaction = b.ReadSingle();
							speedGrass.Filename = Encoding.Default.GetString(b.ReadBytes(b.ReadInt32()));
							var polyshCount = b.ReadInt32();

							for (int p = 0; p < polyshCount; p++)
							{
								var polygon = new Polygon2();
								var pointCount = b.ReadInt32();
								for (int n = 0; n < pointCount; n++)
								{
									var point = new K2DPosition();
									point.X = b.ReadInt32();
									point.Y = b.ReadInt32();
									polygon.Points.Add(point);
								}
								speedGrass.Polygons.Add(polygon);
							}
							DwGrassColony.Add(speedGrass);
						}

	#endregion
					}

					if (Version >= 22)
					{
	#region Event area

						var eventAreaCount = b.ReadInt32();

						for (int i = 0; i < eventAreaCount; i++)
						{
							var area = new EventAreaScript();
							area.AreaId = b.ReadInt32();
							var count = b.ReadInt32();

							for (int p = 0; p < count; p++)
							{
								var polygon = new Polygon2();
								var pointNum = b.ReadInt32();

								for (int n = 0; n < pointNum; n++)
								{
									var point = new K2DPosition();
									point.X = b.ReadInt32();
									point.Y = b.ReadInt32();
									polygon.Points.Add(point);
								}

								area.Polygons.Add(polygon);
							}

							DwEventArea.Add(area);
						}

	#endregion
					}
				}

				XLog.WriteLine(Levels.Good, "Ok");
			}
			catch (Exception exception)
			{
				Blank();
				XLog.WriteLine(Levels.Error, "Failed");
				XLog.WriteLine(Levels.Fatal, "NfmManager::Load<Exception> -> {0}", exception);
			}
		}

		/// <summary>
		/// Get class name to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "nFlavor Map";
		}
	}
}
