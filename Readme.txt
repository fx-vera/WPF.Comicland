Core:
Code necessary to build a new WPF application.

Plugins.
Each plugin (module) is composed by .Models, .Views, .ViewModels.

You can find reused code as RelayCommand following the Microsoft advices.

From nuget:

Fody: 
	It is a library used to notify the changes in the models/viewmodels.
	Using this library you save the work of implement Changes Notifications functionality.
	  
Microsoft.Xaml.Behaviors.Wpf:
	This library is used to allow interactivity with controls. 
	
	

	
The user installer is located in Installer\ExecuteThis

