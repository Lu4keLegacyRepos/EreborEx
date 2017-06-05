using EreborPhoenixExtension.Libs.Extensions.Pathfinding;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs.Skills.Mining
{
	[Serializable]
	public class Mine
	{
		#region Fields 

		private string[] calls = { "You put ", "Nevykopala jsi nic ","Odstranila jsi zaval!", "Odstranil jsi zaval!","Nepovedlo se ti odstranit zaval.", "Jeste nemuzes pouzit skill",                  // 0-4, 5
								   " There is no ore", "too far", "Try mining","Tam nedosahnes.",                    // 6-9
									"afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };     // 10-15


		private string path = Core.Directory + @"\Profiles\XML\";
		private string AlarmPath = Core.Directory + @"\afk.wav";// "C:\\afk.wav";
		private Graphic Ore = 0x19B7;
		private Dictionary<string, UOColor> Material = new Dictionary<string, UOColor>() { { "Copper", 0x099A }, { "Iron", 0x0763 }, { "Kremicity", 0x0481 }, { "Verite", 0x097F }, { "Valorite", 0x0985 }, { "Obsidian", 0x09BD }, { "Adamantium", 0x0026 } };
		private int[] MaterialsCount = { 0, 0, 0, 0, 0, 0, 0 };
		private DateTime StartMine = DateTime.Now;
		private int actualMapIndex = 0;
		private bool crystalEnabled = true;
		private bool autoStockUp = true;
		private bool dropCopper = true;
		private bool dropIron = false;
		private bool dropSilicon = false;
		private bool dropVerite = false;
		private bool removeObstacles = false;
		private UOItem pickAxe;
		private UOItem mace;
		private UOItem oreBox;
		private UOItem gemBox;
		private int maxObsidian = 5;
		private int maxAdamantium = 2;
		private List<Map> maps;
		private Movement mov;
		private PathFinder pathFinder;
		private SearchParameters searchParams;
		private bool skipCopper = false;
		private bool skipIron = false;
		private bool skipSilicon = false;
		private bool skipVerite = false;
		private bool useBank;
		private uint doorLeft = 0x40000963;
		private uint doorRight = 0x40000962;
		private ushort doorLeftClosedGraphic = 0x0841;
		private ushort doorRightClosedGraphic = 0x0843;
		private Point housePosition = new Point(2729, 3272);
		private Point runePosition = new Point(2732, 3270);
		#endregion

		#region Properties

		public bool DropCopper
		{
			get
			{
				return dropCopper;
			}

			set
			{
				dropCopper = value;
			}
		}

		public bool CrystalEnabled
		{
			get
			{
				return crystalEnabled;
			}

			set
			{
				crystalEnabled = value;
			}
		}

		public bool AutoStockUp
		{
			get
			{
				return autoStockUp;
			}

			set
			{
				autoStockUp = value;
			}
		}

		public bool DropIron
		{
			get
			{
				return dropIron;
			}

			set
			{
				dropIron = value;
			}
		}

		public bool DropSilicon
		{
			get
			{
				return dropSilicon;
			}

			set
			{
				dropSilicon = value;
			}
		}

		public bool DropVerite
		{
			get
			{
				return dropVerite;
			}

			set
			{
				dropVerite = value;
			}
		}

		public uint PickAxe
		{
			get
			{
				uint tmp;
				if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E86))
					tmp = World.Player.Layers[Layer.RightHand];

				else tmp = World.Player.Backpack.AllItems.FindType(0x0E85).Exist
						? World.Player.Backpack.AllItems.FindType(0x0E85).Serial : World.Player.Backpack.AllItems.FindType(0x0E86).Exist
						? World.Player.Backpack.AllItems.FindType(0x0E86).Serial : 0;
				if (tmp != 0)
					pickAxe = new UOItem(tmp);
				return tmp;
			}

			set
			{
				pickAxe = new UOItem(value);
			}
		}

		public uint Mace
		{
			get
			{
				uint tmp;
				if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1406) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x1407))
					tmp = World.Player.Layers[Layer.RightHand];
				else tmp = World.Player.Backpack.AllItems.FindType(0x1406).Exist
				? World.Player.Backpack.AllItems.FindType(0x1406).Serial
				: World.Player.Backpack.AllItems.FindType(0x1407).Exist
				? World.Player.Backpack.AllItems.FindType(0x1407).Serial : 0;
				if (tmp != 0)
					mace = new UOItem(tmp);
				return tmp;
			}

			set
			{
				mace = new UOItem(value);
			}
		}

		public bool RemoveObstacles
		{
			get
			{
				return removeObstacles;
			}

			set
			{
				removeObstacles = value;
			}
		}

		public int MaxObsidian
		{
			get
			{
				return maxObsidian;
			}

			set
			{
				maxObsidian = value;
			}
		}

		public int MaxAdamantium
		{
			get
			{
				return maxAdamantium;
			}

			set
			{
				maxAdamantium = value;
			}
		}


		[XmlArray]
		public List<Map> Maps
		{
			get
			{
				if (maps == null)
				{
					maps = new List<Map>();
				}
				return maps;
			}

			set
			{
				maps = value;
			}
		}

		public bool UseBank
		{
			get { return useBank; }
			set { useBank = value; }
		}

		public uint DoorRight
		{
			get
			{
				return doorRight;
			}

			set
			{
				doorRight = value;
			}
		}

		public uint DoorLeft
		{
			get
			{
				return doorLeft;
			}

			set
			{
				doorLeft = value;
			}
		}

		public ushort DoorLeftClosedGraphic
		{
			get
			{
				return doorLeftClosedGraphic;
			}

			set
			{
				doorLeftClosedGraphic = value;
			}
		}

		public ushort DoorRightClosedGraphic
		{
			get
			{
				return doorRightClosedGraphic;
			}

			set
			{
				doorRightClosedGraphic = value;
			}
		}

		public int ActualMapIndex
		{
			get
			{
				return actualMapIndex;
			}

			set
			{
				actualMapIndex = value;
			}
		}
		public uint OreBox
		{
			get
			{

				UOItem tmp = oreBox ?? new UOItem(0x40034E86);
				return tmp.Serial;
			}

			set
			{
				oreBox = new UOItem(value);
			}
		}

		public uint GemBox
		{
			get
			{
				UOItem tmp = gemBox ?? new UOItem(0x4003CFCA);
				return tmp.Serial;
			}

			set
			{
				gemBox = new UOItem(value);
			}
		}

		public bool SkipCopper
		{
			get
			{
				return skipCopper;
			}

			set
			{
				skipCopper = value;
			}
		}

		public bool SkipIron
		{
			get
			{
				return skipIron;
			}

			set
			{
				skipIron = value;
			}
		}

		public bool SkipSilicon
		{
			get
			{
				return skipSilicon;
			}

			set
			{
				skipSilicon = value;
			}
		}

		public bool SkipVerite
		{
			get
			{
				return skipVerite;
			}

			set
			{
				skipVerite = value;
			}
		}

		public Point HousePosition
		{
			get
			{
				return housePosition;
			}

			set
			{
				housePosition = value;
			}
		}

		public Point RunePosition
		{
			get
			{
				return runePosition;
			}

			set
			{
				runePosition = value;
			}
		}

		#endregion


		public Mine()
		{
			mov = new Movement();
			maps = new List<Map>();
			searchParams = new SearchParameters(new Point(), new Point(), null);
			// Defaultne Skalni
			ActualMapIndex = 1;

		}


		
		public void AddMap(string Name)
		{
			var tmp = new Map();
			tmp.Record(Name, Main.Instance.Settings.Mining.Maps.Count());
			Main.Instance.Settings.Mining.Maps.Add(tmp);
		}



		/// <summary>
		/// Check AFK, Attack, Weight etc..
		/// </summary>
		/// <returns>True - MineField is empty</returns>
		public bool Check()
		{try
			{
				bool rtrnTmp = false;
				// Check AFK
				if (Journal.Contains(true, calls[10], calls[11], calls[12], calls[13], calls[14]))
				{
					System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
					my_wave_file.PlaySync();
				}
				// Check CK/Monster
				foreach (UOCharacter ch in World.Characters)
				{
					if (ch.Notoriety > Notoriety.Criminal)
					{
						System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
						my_wave_file.Play();
						Battle b = new Battle(MoveTo, MoveToFarestField, moveXField, ch, mace);
						b.Kill();
					}
				}

				// Check Light
				if (Journal.Contains(true, calls[15]) || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
				{
					World.Player.Layers[Layer.LeftHand].Use();
					UO.Wait(200);
					if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

				}
				// Count materials
				for (int o = 0; o < Material.Count; o++)
				{
					int tmp;
					switch (o)
					{
						case 0:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Copper"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 1:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Iron"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 2:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Kremicity"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 3:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Verite"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 4:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Valorite"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 5:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Obsidian"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;
						case 6:
							tmp = World.Player.Backpack.Items.FindType(Ore, Material["Adamantium"]).Amount;
							MaterialsCount[o] = tmp > 0 ? tmp : 0;
							break;

					}
				}
				Main.Instance.EreborInstance.Invoke(new MethodInvoker(delegate
				{
					Main.Instance.EreborInstance.CopperCount = MaterialsCount[0].ToString();
					Main.Instance.EreborInstance.IronCount = MaterialsCount[1].ToString();
					Main.Instance.EreborInstance.SiliconCount = MaterialsCount[2].ToString();
					Main.Instance.EreborInstance.VeriteCount = MaterialsCount[3].ToString();
					Main.Instance.EreborInstance.ValoriteCount = MaterialsCount[4].ToString();
					Main.Instance.EreborInstance.ObsidianCount = MaterialsCount[5].ToString();
					Main.Instance.EreborInstance.AdamantiumCount = MaterialsCount[6].ToString();
				}));

				// No Ore
				if (Journal.Contains(true, calls[6], calls[7], calls[8], calls[9]))
					rtrnTmp = true;

				// Skill delay
				if (Journal.Contains(true, calls[5]))
				{
					UO.Wait(5000);
				}

				// Check Weight
				if (World.Player.Weight > (World.Player.Strenght * 4 + 15))
				{
					Unload();
					return false;
				}

				// Incoming Ore  
				if (Journal.Contains(true, calls[0], calls[1], calls[2], calls[3], calls[4]))
				{

					if (Journal.Contains(true, " zaval!"))
					{
						rtrnTmp = true;
					}
					if (Journal.Contains(true, "Copper "))
					{
						if (SkipCopper || SkipIron || SkipSilicon || SkipVerite)
							rtrnTmp = true;
						if (DropCopper)
						{
							World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).DropHere(ushort.MaxValue);
							rtrnTmp = true;
						}
					}
					if (Journal.Contains(true, "Iron "))
					{
						if (SkipCopper || SkipIron || SkipSilicon || SkipVerite)
							rtrnTmp = true;
						if (DropIron)
						{
							World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).DropHere(ushort.MaxValue);
							rtrnTmp = true;
						}
					}
					if (Journal.Contains(true, "Kremicity "))
					{
						if (SkipCopper || SkipIron || SkipSilicon || SkipVerite)
							rtrnTmp = true;
						if (DropSilicon)
						{
							World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).DropHere(ushort.MaxValue);
							rtrnTmp = true;
						}
					}
					if (Journal.Contains(true, "Verite "))
					{
						if (SkipCopper || SkipIron || SkipSilicon || SkipVerite)
							rtrnTmp = true;
						if (DropVerite)
						{
							World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).DropHere(ushort.MaxValue);
							rtrnTmp = true;
						}
					}



				}
				Journal.Clear();
				if (rtrnTmp)
				{
					// Check amount of Best materials
					if (MaterialsCount[5] >= MaxObsidian)
					{
						Unload();
					}
					else
					if (MaterialsCount[6] >= MaxAdamantium)
					{
						Unload();
					}
				}
				return rtrnTmp;
			}
			catch { return true; };




		}


		public void Work()
		{
			while (true)
			{
				MineHere(MoveToClosestExploitable(), 0);
				UO.Wait(200);
				if (RemoveObstacles)
				{
					while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200)) UO.Wait(100);
					Maps[ActualMapIndex].RemoveNearObstacles(MineHere);
				}
			}

		}


		private bool CheckTools()
		{
			Mace = Mace;
			PickAxe = PickAxe;

			if (Mace == 0)
			{
				Unload();
				return false;
			}
			if (PickAxe == 0)
			{
				Unload();
				return false;
			}
			return true;
		}


	
		private MineField MoveToClosestExploitable()
		{
			Maps[ActualMapIndex].FindObstacles();
			Maps[ActualMapIndex].Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
			MineField tmp;
			try
			{
				tmp = Maps[ActualMapIndex].Fields.First(x => x.State == MineFieldState.Unknown);
				MoveTo(tmp.Location);
			}
			catch
			{
				tmp = null;
				UO.PrintError("Nenalezen tezitelbne pole");
			}
			return tmp;
		}


		public void MoveTo(int X, int Y)
		{
			foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), new Point(X, Y)))
			{
				mov.moveToPosition(p);
			}
		}


		public void MoveTo(Point location)
		{
			foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), location))
			{
				mov.moveToPosition(p);
			}
		}


		private List<Point> GetWay(Point StartPosition, Point EndPosition)
		{
			searchParams = new SearchParameters(StartPosition, EndPosition, Maps[ActualMapIndex]);
			pathFinder = new PathFinder(searchParams);
			return pathFinder.FindPath();
		}


		public void MoveToFarestField()
		{
			Maps[ActualMapIndex].FindObstacles();
			Maps[ActualMapIndex].Fields.Sort((b, a) => a.Distance.CompareTo(b.Distance));
			MineField tmp;
			try
			{
				tmp = Maps[ActualMapIndex].Fields.First(x => x.IsExploitable);
				MoveTo(tmp.Location);
			}
			catch
			{
				tmp = null;
				UO.PrintError("Nenalezen tezitelbne pole");
			}
		}

		public void moveXField(int distance)
		{
			Maps[ActualMapIndex].FindObstacles();
			Maps[ActualMapIndex].Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
			MineField tmp;
			try
			{
				tmp = Maps[ActualMapIndex].Fields.First(x => x.Distance>=distance);
				MoveTo(tmp.Location);
			}
			catch
			{
				tmp = null;
				UO.PrintError("Nenalezen tezitelbne pole");
			}
		}


		public void Unload()
		{
			Point ActualPosition = new Point(World.Player.X, World.Player.Y);
			int tmpMapIndex = ActualMapIndex;
			Recall(0);
			UOItem dltmp = new UOItem(DoorLeft);
			UOItem drtmp = new UOItem(DoorRight);
			if (dltmp.Graphic == DoorLeftClosedGraphic) dltmp.Use();
			if (drtmp.Graphic == DoorRightClosedGraphic) drtmp.Use();
			StockUp();
			ActualMapIndex = tmpMapIndex;
			Recall(1);

		}


		private void Recall(int v)
		{

			int x = World.Player.X;
			int y = World.Player.Y;
			UO.Warmode(false);
			switch (v)
			{
				case 0:
					while (World.Player.X == x | World.Player.Y == y)
					{
						while (World.Player.Mana < 20)
						{
							UO.UseSkill(StandardSkill.Meditation);
							UO.Wait(2500);
						}
						UO.WaitTargetSelf();
						UO.Say(".recallhome");
						Journal.WaitForText(true, 11000, "Kouzlo se nezdarilo.");
						Journal.ClearAll();
						UO.Wait(200);
					}
					break;
				case 1:
					//while (World.Player.X == x || World.Player.Y == y)
					//{
					//	while (World.Player.Mana < 20)
					//	{
					//		UO.UseSkill(StandardSkill.Meditation);
					//		UO.Wait(500);
					//	}

					//	foreach (Runes.Rune r in Main.Instance.Settings.RuneTree.Runes.Where(a => a.Name == "Skalni dul"))
					//	{
					//		Main.Instance.Settings.RuneTree.findRune(r);
					//		r.RecallSvitek();
					//	}


					//	Journal.ClearAll();
					//	UO.Wait(500);
					//	Journal.WaitForText(true, 12000, "Kouzlo se nezdarilo.");
					//	UO.Wait(200);
					//}



					while (World.Player.X == x || World.Player.Y == y)
					{
						while (World.Player.Mana < 20)
						{
							UO.UseSkill(StandardSkill.Meditation);
							UO.Wait(500);
						}

						foreach (Runes.Rune r in Main.Instance.Settings.RuneTree.Runes.Where
							(a => a.Name == Main.Instance.Settings.Mining.Maps[Main.Instance.Settings.Mining.ActualMapIndex].Name))
						{
							Main.Instance.Settings.RuneTree.findRune(r);
							r.RecallSvitek();
						}


						Journal.ClearAll();
						UO.Wait(500);
						Journal.WaitForText(true, 12000, "Kouzlo se nezdarilo.");
						UO.Wait(200);
					}
					break;
			}


		}


		private void StockUp()
		{
			ActualMapIndex = 0;
			MoveTo(HousePosition);
			if (!AutoStockUp) UO.TerminateAll();
			MoveOre_Feed_GetRecall();
			MoveTo(RunePosition);

		}


		private void MoveOre_Feed_GetRecall()
		{
			Serial tmp;
			if (UseBank) openBank(14);
			UOItem box = new UOItem(OreBox);
			box.Use();
			UO.Wait(200);
			new UOItem(GemBox).Use();
			World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Valorite"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Obsidian"]).Move(ushort.MaxValue, OreBox);
			World.Player.Backpack.AllItems.FindType(Ore, Material["Adamantium"]).Move(ushort.MaxValue, OreBox);
			UO.Wait(100);
			if (PickAxe == 0)
			{

				tmp = box.AllItems.FindType(0x1406).Exist
						? box.AllItems.FindType(0x1406).Serial : box.AllItems.FindType(0x1407).Exist
						? box.AllItems.FindType(0x1407).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas krumpac");
					UO.TerminateAll();
				}
			}
		
			if (Mace == 0)
			{
				tmp = box.AllItems.FindType(0x0E85).Exist
				? box.AllItems.FindType(0x0E85).Serial : box.AllItems.FindType(0x0E86).Exist
				? box.AllItems.FindType(0x0E86).Serial : 0;
				if (tmp == 0)
				{
					UO.PrintError("Nemas Zbran");
					UO.TerminateAll();
				} 
}
			for (ushort i = 0x0F0F; i < 0x0F31; i++)
			{
				World.Player.Backpack.AllItems.FindType(i).Move(ushort.MaxValue, GemBox);
			}
			// Mramor
			while (World.Player.Backpack.AllItems.FindType(0x1363).Amount > 0)
				World.Player.Backpack.AllItems.FindType(0x1363).Move(ushort.MaxValue, OreBox);



			SelfFeed();
			box.Use();
			if (World.Player.Backpack.AllItems.FindType(0x1F4C).Amount < 4)
				box.AllItems.FindType(0x1F4C).Move(7, World.Player.Backpack);

		}


		private void SelfFeed()
		{
			World.FindDistance = 4;
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
			World.Ground.FindType(0x097B).Use();
			UO.Wait(100);
		}


		private void MineHere(MineField mf, int Try)
		{
			if (mf == null)
			{
				return;
			}

			while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200))
			{
				UO.Wait(50);
				if (Check())
				{
					mf.State = MineFieldState.Empty;
					XmlSerializeHelper<Mine>.Save("Mining", Main.Instance.Settings.Mining);
					return;
				}
			}

			if (!CheckTools()) return;


			if (CrystalEnabled && Try == 0)
			{
				UO.Say(".vigour");
				UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
				(pickAxe).Use();
				StartMine = DateTime.Now;
				Journal.WaitForText(true, 1500, "Nasla jsi lepsi material!", "Nasel jsi lepsi material!");
				UO.Say(".vigour");
				MineHere(mf, Try + 1);
			}
			else
			{
				UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
				(pickAxe).Use();
				StartMine = DateTime.Now;
				UO.Wait(100);
				MineHere(mf, Try + 1);
			}
		}

		private void MineHere(UOItem item, int Try)
		{
			if (item == null)
			{
				return;
			}

			while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200))
			{
				UO.Wait(50);
				if (!RemoveObstacles) return;
				if (Check())
				{
					return;
				}
			}

			if (!CheckTools()) return;

			{
				try
				{
					item.WaitTarget();
					(pickAxe).Use();
				}
				catch { }
				StartMine = DateTime.Now;
				UO.Wait(100);
				MineHere(item, Try + 1);
			}
		}


		public void SelectMap(int index)
		{
			if (index >= 0 && index >= Maps.Count) return;
			ActualMapIndex = index;
		}


		public void fillListBox(ListBox lb)
		{
			lb.Items.Clear();
			foreach (Map e in Maps)
			{
				lb.Items.Add(e.Name);
			}

		}


		private void openBank(int equip)
		{
			Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
			Core.RegisterServerMessageCallback(0xB0, onGumpBank);
			UO.Say(".equip{0}", equip);
			UO.Wait(600);
			Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
		}


		private CallbackResult onGumpBank(byte[] data, CallbackResult prevResult)
		{
			byte cmd = 0xB1; //1 byte
			uint ID, gumpID;
			uint buttonID = 9; //4 byte
			uint switchCount = 0;
			uint textCount = 0;

			PacketReader pr = new PacketReader(data);
			if (pr.ReadByte() != 0xB0) return CallbackResult.Normal;
			pr.ReadInt16();
			ID = pr.ReadUInt32();
			gumpID = pr.ReadUInt32();


			PacketWriter reply = new PacketWriter();
			reply.Write(cmd);
			reply.WriteBlockSize();
			reply.Write(ID);
			reply.Write(gumpID);
			reply.Write(buttonID);
			reply.Write(switchCount);
			reply.Write(textCount);

			Core.SendToServer(reply.GetBytes());
			return CallbackResult.Sent;
		}
	}
}
