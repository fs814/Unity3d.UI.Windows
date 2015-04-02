//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the UI.Windows Flow Addon.
//     You may simply edit this file to setup your behavior.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI.Windows;
using UnityEngine.UI.Windows.Types;

namespace ExampleProject.UI.Menu.GameTypeChooser {

	public class GameTypeChooserScreenBase : LayoutWindowType {
		
		/// <summary>
		/// Flows to the Shop.
		/// Full Name: ExampleProject.UI.Menu.ShopOperations.Shop.ShopScreen
		/// </summary>
		/// <returns>Shop</returns>
		public virtual ExampleProject.UI.Menu.ShopOperations.Shop.ShopScreen FlowShop(params object[] parameters) {
			
			return WindowSystem.Show<ExampleProject.UI.Menu.ShopOperations.Shop.ShopScreen>(parameters);
			
		}
				
		/// <summary>
		/// Flows to the GameplayLoader.
		/// Full Name: ExampleProject.UI.Other.GameplayLoader.GameplayLoaderScreen
		/// </summary>
		/// <returns>GameplayLoader</returns>
		public virtual ExampleProject.UI.Other.GameplayLoader.GameplayLoaderScreen FlowGameplayLoader(params object[] parameters) {
			
			return WindowSystem.Show<ExampleProject.UI.Other.GameplayLoader.GameplayLoaderScreen>(parameters);
			
		}
		
	}

}