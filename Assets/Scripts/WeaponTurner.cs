using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Weapon : IComparable<Weapon>
{
    public enum WeaponType { Knife, Sword, Axe }

    public WeaponType type;
    public float attackSpeed;
    public float damage;
    public float damagePerSecond => attackSpeed != 0 ? damage / attackSpeed : 0;

    public Weapon(WeaponType type, float damage, float attackSpeed)
    {
        this.type = type;
        this.damage = damage;
        this.attackSpeed = attackSpeed;
    }

    public string Serialize()
    {
        string result = $"{damage.ToString("0.0")}, {attackSpeed.ToString("0.00")}, {type.ToString()}";
        return result;
    }

    public static Weapon Deserialize(string serializedWeapon)
    {
        if (!string.IsNullOrEmpty(serializedWeapon))
        {
            serializedWeapon.Trim('\0', ' ', '\t'); // remove spaces and hidden inavlid characters
            string[] values = serializedWeapon.Split(','); // separate the string into an array
            bool valid = values.Length > 2; // if we found at least 3 values, i consider that we do have a valid serialized weapon so far at least 
            if (valid)
            {
                float damage;
                float attackSpeed;
                WeaponType type;

                valid &= float.TryParse(values[0], out damage);
                valid &= float.TryParse(values[1], out attackSpeed);
                valid &= System.Enum.TryParse(values[2], out type);

                if (valid) // at this point if valid is true we were able to parse all values.
                {
                    Weapon result = new Weapon(type, damage, attackSpeed);
                    return result;
                }
            }
        }
        return null;
    }

    public int CompareTo(Weapon other)
    {
        int result = this.damagePerSecond.CompareTo(other.damagePerSecond);
        return result;
    }
}
public class SortWeaponTypeByTypeName : IComparer<Weapon.WeaponType>
{
    public int Compare(Weapon.WeaponType x, Weapon.WeaponType y)
    {
        return x.ToString().CompareTo(y.ToString());
    }
}

public class WeaponTurner
{
    private static int numberOfWeaponsToKeep = 3;
    public static void TuneWeapons(string input_file_path, string output_file_path)
    {
        string file_raw = ReadFile(input_file_path);
        string errorID = "Error";
        bool validFile = !string.IsNullOrEmpty(file_raw) && file_raw.Length >= errorID.Length && file_raw.Substring(0, errorID.Length) != errorID;
        if (validFile)
        {
            string[] rows = file_raw.Split('\n'); // separate the file in an array of strings the separation is done in the \n (new line / return)
            if (rows.Length > 1)
            {
                string headerRow = rows[0];
                List<Weapon> weapons = new List<Weapon>(); // the list that will hold all found and successfuly parsed weapons
                for (int i = 1; i < rows.Length; i++) // notice i starts with 1. because we want to skip the header
                {
                    Weapon weapon = Weapon.Deserialize(rows[i]); // attempts to parse the weapon from the row
                    if (weapon != null) //if it succeded
                    {
                        weapons.Add(weapon); // we store it :)
                    }
                }

                // we by now should have all weapons listed in 'weapons' and we proceed to organize and discard 
                Weapon.WeaponType[] allWeaponTypes = (Weapon.WeaponType[])System.Enum.GetValues(typeof(Weapon.WeaponType)); // get an array of all the known weapon types.
                List<Weapon.WeaponType> allWeaponTypesOrganizer = new List<Weapon.WeaponType>(allWeaponTypes);
                SortWeaponTypeByTypeName sortWeaponTypesByName = new SortWeaponTypeByTypeName();
                allWeaponTypesOrganizer.Sort(sortWeaponTypesByName); // sort the weapon types by alphabetic name
                allWeaponTypes = allWeaponTypesOrganizer.ToArray(); // back to array, not neccesary but I prefer for loops when they are used with arrays.

                List<Weapon> result = new List<Weapon>(); // lets store all refined weapons here. we will be storing them sorted already

                for (int i = 0; i < allWeaponTypes.Length; i++)
                {
                    Weapon.WeaponType type = allWeaponTypes[i];
                    List<Weapon> weaponsMatchingThatType = weapons.FindAll((w) => w.type == type); // find in our list of weapons those who match the weapon type.
                    weaponsMatchingThatType.Sort(); // sorts by damage per second, in ascending order, defined in the weapon class 'CompareTo'
                    weaponsMatchingThatType.Reverse(); // reverse the order so the first element is the most powerful
                    if (weaponsMatchingThatType.Count > numberOfWeaponsToKeep)
                    {
                        weaponsMatchingThatType.RemoveRange(numberOfWeaponsToKeep, weaponsMatchingThatType.Count - numberOfWeaponsToKeep); // remove excess after sorted.
                    }
                    result.AddRange(weaponsMatchingThatType);
                }

                // by this point we have already organize all and got rid of the excees weaker weapons, time to prepare for printing back
                string serializedResult = headerRow; // we start with the header
                for (int i = 0; i < result.Count; i++)
                {
                    Weapon weapon = result[i];
                    string weaponDataSerialized = weapon.Serialize();
                    serializedResult += $"\n{weaponDataSerialized}";
                }

                bool succed = WriteFile(output_file_path, serializedResult);
                if (succed)
                {
                    // yay we did it.
                }
            }
        }
    }

    public static string ReadFile(string fullPath) // must include extention of file
    {
        string directoryPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directoryPath) || !File.Exists(fullPath)) { return "Error, file at (" + fullPath + ") couldn't be found."; }
        string result = "";
        try
        {
            result = File.ReadAllText(fullPath);
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error" + e.Message);
            return e.Message;
        }
    }

    public static bool WriteFile(string fullPath, string content) // must include extention of file
    {
        string directoryPath = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        try
        {
            File.WriteAllText(fullPath, content);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
}
