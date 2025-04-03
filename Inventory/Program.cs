using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Item Module
public class Item
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public string GetItemDetails()
    {
        return $"ID: {ItemId}, Name: {Name}, Quantity: {Quantity}, Price: {Price:C}";
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity >= 0)
        {
            Quantity = newQuantity;
        }
        else
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }
    }
}

// Inventory Module
public class Inventory
{
    private List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(int itemId)
    {
        Item itemToRemove = items.Find(item => item.ItemId == itemId);
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
        }
        else
        {
            throw new KeyNotFoundException($"Item with ID {itemId} not found.");
        }
    }

    public Item GetItem(int itemId)
    {
        return items.Find(item => item.ItemId == itemId);
    }

    public List<Item> GetAllItems()
    {
        return items;
    }

    public decimal CalculateTotalValue()
    {
        decimal totalValue = 0;
        foreach (var item in items)
        {
            totalValue += item.Quantity * item.Price;
        }
        return totalValue;
    }

    public void SaveInventory(string filePath)
    {
        string jsonString = JsonSerializer.Serialize(items);
        File.WriteAllText(filePath, jsonString);
    }

    public void LoadInventory(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            items = JsonSerializer.Deserialize<List<Item>>(jsonString) ?? new List<Item>();
        }
        else
        {
            items = new List<Item>(); // Ensure list is initialized even if file doesn't exist.
        }
    }
}

// UI Module (Command-Line)
public class UI
{
    private Inventory inventory;
    private string filePath = "inventory.json";

    public UI(Inventory inventory)
    {
        this.inventory = inventory;
        this.inventory.LoadInventory(filePath);
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\nInventory Management System");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. Remove Item");
            Console.WriteLine("3. View Item");
            Console.WriteLine("4. View All Items");
            Console.WriteLine("5. Calculate Total Value");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddItem();
                    break;
                case "2":
                    RemoveItem();
                    break;
                case "3":
                    ViewItem();
                    break;
                case "4":
                    ViewAllItems();
                    break;
                case "5":
                    CalculateTotalValue();
                    break;
                case "6":
                    inventory.SaveInventory(filePath);
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private void AddItem()
    {
        Console.Write("Enter Item ID: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Enter Item Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Quantity: ");
        int quantity = int.Parse(Console.ReadLine());
        Console.Write("Enter Price: ");
        decimal price = decimal.Parse(Console.ReadLine());

        Item newItem = new Item { ItemId = id, Name = name, Quantity = quantity, Price = price };
        inventory.AddItem(newItem);
        Console.WriteLine("Item added.");
    }

    private void RemoveItem()
    {
        Console.Write("Enter Item ID to remove: ");
        int id = int.Parse(Console.ReadLine());
        try
        {
            inventory.RemoveItem(id);
            Console.WriteLine("Item removed.");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ViewItem()
    {
        Console.Write("Enter Item ID to view: ");
        int id = int.Parse(Console.ReadLine());
        Item item = inventory.GetItem(id);
        if (item != null)
        {
            Console.WriteLine(item.GetItemDetails());
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }

    private void ViewAllItems()
    {
        List<Item> allItems = inventory.GetAllItems();
        if (allItems.Count > 0)
        {
            foreach (var item in allItems)
            {
                Console.WriteLine(item.GetItemDetails());
            }
        }
        else
        {
            Console.WriteLine("Inventory is empty.");
        }
    }

    private void CalculateTotalValue()
    {
        Console.WriteLine($"Total inventory value: {inventory.CalculateTotalValue():C}");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Inventory inventory = new Inventory();
        UI ui = new UI(inventory);
        ui.Run();
    }
}