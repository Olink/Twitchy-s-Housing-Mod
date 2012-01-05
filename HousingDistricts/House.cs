using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI.DB;
using TShockAPI;
using Terraria;

namespace HousingDistricts
{
    public class House
    {
        public Rectangle HouseArea { get; set; }
        public List<string> Owners { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string WorldID { get; set; }
        public int Locked { get; set; }

        public House(Rectangle housearea, List<string> owners, int id, string name, string worldid, int locked)
        {
            HouseArea = housearea;
            Owners = owners;
            ID = id;
            Name = name;
            WorldID = worldid;
            Locked = locked;
        }
    }

    public class HouseTools
    {
        public static bool AddHouse(int tx, int ty, int width, int height, string housename, int locked)
        {
            List<SqlValue> values = new List<SqlValue>();
            values.Add(new SqlValue("Name", "'" + housename + "'"));
            values.Add(new SqlValue("TopX", tx));
            values.Add(new SqlValue("TopY", ty));
            values.Add(new SqlValue("BottomX", width));
            values.Add(new SqlValue("BottomY", height));
            values.Add(new SqlValue("Owners", "0"));
            values.Add(new SqlValue("WorldID", "'" + Main.worldID.ToString() + "'"));
            values.Add(new SqlValue("Locked", locked));
            HousingDistricts.SQLEditor.InsertValues("HousingDistrict", values);
            HousingDistricts.Houses.Add(new House(new Rectangle(tx, ty, width, height), new List<string>(), (HousingDistricts.Houses.Count + 1), housename, Main.worldID.ToString(), locked));
            return true;
        }

        public static bool AddNewUser(string houseName, string id)
        {
            var house = GetHouseByName(houseName);
            StringBuilder sb = new StringBuilder();
            int count = 0;
            house.Owners.Add(id);
            foreach(string owner in house.Owners)
            {
                count++;
                sb.Append(owner);
                if(count != house.Owners.Count)
                    sb.Append(",");
            }
            List<SqlValue> values = new List<SqlValue>();
            values.Add(new SqlValue("Owners", "'" + sb.ToString() + "'"));

            List<SqlValue> wheres = new List<SqlValue>();
            wheres.Add(new SqlValue("Name", "'" + houseName + "'"));

            HousingDistricts.SQLEditor.UpdateValues("HousingDistrict", values, wheres);
            return true;
        }

        public static bool ChangeLock(string houseName)
        {
            var house = GetHouseByName(houseName);
            bool locked = false;

            if (house.Locked == 0)
                locked = true;
            else
                locked = false;

            house.Locked = locked ? 1 : 0;

            List<SqlValue> values = new List<SqlValue>();
            values.Add(new SqlValue("Locked", locked ? 1 : 0));
            HousingDistricts.SQLEditor.UpdateValues("HousingDistrict", values, new List<SqlValue>());
            return locked;
        }

        public static House GetHouseByName(string name)
        {
            foreach (House house in HousingDistricts.Houses)
            {
                if (house.Name == name)
                    return house;
            }
            return null;
        }
    }
}
