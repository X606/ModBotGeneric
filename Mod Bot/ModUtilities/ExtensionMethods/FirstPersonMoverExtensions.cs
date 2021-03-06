using System;
using System.Collections.Generic;
using System.Linq;

namespace ModLibrary
{
    /// <summary>
    /// Defines extension methods for <see cref="FirstPersonMover"/>
    /// </summary>
    public static class FirstPersonMoverExtensions
    {
        /// <summary>
        /// Returns the <see cref="FirstPersonMover"/>s currently equipped weapon, will return null if the <see cref="CharacterModel"/> is <see langword="null"/>, or the currently equipped <see cref="WeaponType"/> is <see cref="WeaponType.None"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <returns></returns>
        public static WeaponModel GetEquippedWeaponModel(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover.HasCharacterModel() || firstPersonMover.GetEquippedWeaponType() == WeaponType.None)
                return null;

            WeaponType equippedWeaponType = firstPersonMover.GetEquippedWeaponType();
            return firstPersonMover.GetCharacterModel().GetWeaponModel(equippedWeaponType);
        }

        /// <summary>
        /// Gives the specified upgrade to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <param name="upgradeType">The <see cref="UpgradeType"/> to give</param>
        /// <param name="level">The level of the upgrade</param>
        /// <exception cref="ArgumentNullException">If the given <see cref="FirstPersonMover"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">If the given <see cref="UpgradeType"/> and level has not been defined in <see cref="UpgradeManager.UpgradeDescriptions"/></exception>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeType upgradeType, int level)
        {
            if (firstPersonMover == null)
                throw new ArgumentNullException(nameof(firstPersonMover));

            if (UpgradeManager.Instance.GetUpgrade(upgradeType, level) == null)
                throw new ArgumentException("The upgrade with type \"" + upgradeType + "\" and level " + level + " has not been defined!");

            if (firstPersonMover.GetComponent<PreconfiguredUpgradeCollection>() != null) // If we are giving an upgrade to an enemy/ally
            {
                PreconfiguredUpgradeCollection upgradeCollection = firstPersonMover.GetComponent<PreconfiguredUpgradeCollection>();
                UpgradeTypeAndLevel upgradeToGive = new UpgradeTypeAndLevel { UpgradeType = upgradeType, Level = level };

                List<UpgradeTypeAndLevel> upgrades = upgradeCollection.Upgrades.ToList();
                upgrades.Add(upgradeToGive);
                upgradeCollection.Upgrades = upgrades.ToArray();

                upgradeCollection.InitializeUpgrades();

                firstPersonMover.RefreshUpgrades();
            }
            else if (firstPersonMover.GetComponent<PlayerUpgradeCollection>() != null) // If we are giving it to the player
            {
                GameDataManager.Instance.SetUpgradeLevel(upgradeType, level); // Set the level of the upgrade to the given one
                UpgradeDescription upgrade = UpgradeManager.Instance.GetUpgrade(upgradeType, level);
                GlobalEventManager.Instance.Dispatch(GlobalEvents.UpgradeCompleted, upgrade);
            }

            firstPersonMover.SetUpgradesNeedsRefreshing();
        }

        /// <summary>
        /// Gives the specified <see cref="UpgradeDescription"/> to a <see cref="FirstPersonMover"/>
        /// </summary>
        /// <param name="firstPersonMover"></param>
        /// <param name="Upgrade">The upgrade to give</param>
        public static void GiveUpgrade(this FirstPersonMover firstPersonMover, UpgradeDescription Upgrade)
        {
            firstPersonMover.GiveUpgrade(Upgrade.UpgradeType, Upgrade.Level);
        }
    }
}
