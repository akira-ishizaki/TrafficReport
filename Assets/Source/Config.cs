using System;
using UnityEngine;
using TrafficReport;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using TrafficReport.Util;

namespace TrafficReport
{	
	
	[Serializable]
	public class VehicleDisplay {

        [XmlAttribute]
		public string id;
        [XmlAttribute]
		public string display;
        [XmlAttribute]
		public bool onOff;
        [XmlAttribute]
        public Color32 color;
	}

    public delegate void ConfigChangeDelegate();

	[Serializable]
	public class Config {

        public event ConfigChangeDelegate eventConfigChanged;

        public static int CONFIG_VERSION = 2;

        public int configVersion = -1;

        public static Config instance = Config.Load();
        public Vector3 pos = new Vector3(58, 8);

		public KeyCode keyCode = KeyCode.Slash;


        [XmlArray(ElementName = "VehicleTypes")]
        [XmlArrayItem(ElementName = "VehicleType")]
        private VehicleDisplay[] vehicleTypes = {
			new VehicleDisplay { id =  "Citizen/Foot", display = "歩行者", onOff=true, color = new Color32(1,255,216,255) },
            new VehicleDisplay { id =  "Citizen/Cycle", display = "サイクリスト", onOff=true, color = new Color32(1,255,216,255) },
			
			new VehicleDisplay { id =  "Citizen/Car", display = "車", onOff=true , color = new Color32(35,100,90,255) },
            new VehicleDisplay { id =  "Citizen/Scooter", display = "スクーター", onOff=true , color = new Color32(70,120,100,255) },
			
			new VehicleDisplay { id =  "Industrial/IndustrialGeneric", display = "貨物トラック", onOff=true , color = new Color32(154,92,59,255)  },
			new VehicleDisplay { id =  "Industrial/IndustrialOil", display = "石油トレーラー", onOff=true , color = new Color32(46,39,35,255) },
			new VehicleDisplay { id =  "Industrial/IndustrialOre", display = "鉱石トレーラー", onOff=true  , color = new Color32(150,144,141,255)},
			new VehicleDisplay { id =  "Industrial/IndustrialForestry", display = "木材トレーラー", onOff=true , color = new Color32(140,39,16,255) },
			new VehicleDisplay { id =  "Industrial/IndustrialFarming", display = "トラクター", onOff=true , color = new Color32(148,86,52,255) },
			
			new VehicleDisplay { id =  "HealthCare/None", display = "救急車", onOff=true  , color = new Color32(30,255,0,255) },
            new VehicleDisplay { id =  "DeathCare/None", display = "霊柩車", onOff=true  , color = new Color32(30,180,0,255) },
			new VehicleDisplay { id =  "Garbage/None", display = "ゴミ収集車", onOff=true , color = new Color32(255,240,0,255)  },
			new VehicleDisplay { id =  "PoliceDepartment/None", display = "パトカー", onOff=true , color = new Color32(24,19,249,255)   },
			new VehicleDisplay { id =  "FireDepartment/None", display = "消防車", onOff=true , color = new Color32(255,0,0,255)  },
			
            new VehicleDisplay { id =  "PublicTransport/PublicTransportMetro", display = "地下鉄", onOff=true, color = new Color32(255,150,0,255)  },
            new VehicleDisplay { id =  "PublicTransport/PublicTransportTrain", display = "鉄道", onOff=true, color = new Color32(255,150,0,255)  },
            
			new VehicleDisplay { id =  "PublicTransport/PublicTransportBus", display = "バス", onOff=true, color = new Color32(170,57,249,255)  },
            new VehicleDisplay { id =  "PublicTransport/PublicTransportTaxi", display = "タクシー", onOff=true, color = new Color32(100,57,249,255)  }
	    };

        public void NotifyChange()
        {
            Save();
            if(eventConfigChanged!=null)
                eventConfigChanged();
        }
		
		public static Config Load() {
			try {
				XmlSerializer xml = new XmlSerializer (typeof(Config));
				FileStream fs = new FileStream("TrafficReport.xml", FileMode.Open, FileAccess.Read);
				Config config =  xml.Deserialize(fs) as Config;
                fs.Close();
                if (config.configVersion != CONFIG_VERSION)
                {
                    Config c = new Config();
                    c.configVersion = CONFIG_VERSION;
                    return c;
                }

      			return config;
			} catch {
				return new Config();
			} 
		}
		
		public void Save() {
			try 
			{
				XmlSerializer xml = new XmlSerializer (GetType());
				FileStream fs = new FileStream ("TrafficReport.xml", FileMode.Create, FileAccess.Write);
				xml.Serialize (fs,this);
				fs.Close();
			} catch(Exception e) {
				Log.error("Error saving config" + e.ToString());

			}
		}

        internal bool IsTypeVisible(string p)
        {
            foreach(VehicleDisplay v in vehicleTypes) {
                if(v.id == p) {
                    return v.onOff;
                }
            }
            return true;
        }


        internal void ToggleVisibility(string p)
        {
            foreach (VehicleDisplay v in vehicleTypes)
            {
                if (v.id == p)
                {
                    v.onOff = !v.onOff;
                }
            }
            NotifyChange();
        }

        internal Color32 GetTypeColor(string type)
        {
            foreach (VehicleDisplay v in vehicleTypes)
            {
                if (v.id == type)
                {
                    return v.color;
                }
            }
            return new Color32();
        }

        internal String GetTypeDisplay(string type)
        {
            foreach (VehicleDisplay v in vehicleTypes)
            {
                if (v.id == type)
                {
                    return v.display;
                }
            }
            return type;
        }
    }
}

