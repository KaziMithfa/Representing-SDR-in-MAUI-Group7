namespace Contacts.Maui.Views;

public partial class ContactsPage : ContentPage
{
	public ContactsPage()
	{
		InitializeComponent();

        List<string> contacts = new List<string>() {
            "John Doe",
            "Jane Doe",
            "Tom Hanks",
            "Frank Liu"};

        listContacts.ItemsSource = contacts;


    }
	
    
}