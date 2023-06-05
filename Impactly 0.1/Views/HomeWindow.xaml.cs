using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Impactly_0._1.@class;
using Newtonsoft.Json;

namespace Impactly_0._1.Views
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Todo> todoList = new ObservableCollection<Todo>();

        public ObservableCollection<Todo> TodoList
        {
            get { return todoList; }
            set
            {
                todoList = value;
                OnPropertyChanged(nameof(TodoList));
            }
        }
        public Todo SelectedTodo { get; set; }
        private readonly AmazonDynamoDBClient _client;
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = this;
            var accessKey = "AKIAVWGNXKC66GE47MFB";
            var secretKey = "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";
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "https://dynamodb.eu-north-1.amazonaws.com"
            };
            _client = new AmazonDynamoDBClient(accessKey, secretKey, config);
            LoadTodoListFromDynamoDB();
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ShowAddTodoDialog(out var newTodo))
            {
                TodoList.Add(newTodo);

                string todoListJson = JsonConvert.SerializeObject(TodoList);
                PutItemRequest request = new PutItemRequest
                {
                    TableName = "todos",
                    Item = new Dictionary<string, AttributeValue>
                    {
                        { "userid", new AttributeValue { S = UserSession.UserId } }, 
                        { "TodoListJson", new AttributeValue { S = todoListJson } }
                    }
                };

                await _client.PutItemAsync(request);

                RefreshTodoList();
            }
        }

        private bool ShowAddTodoDialog(out Todo newTodo)
        {

            var dialog = new EditTodoDialogWindow();
            dialog.DataContext = dialog;

            var result = dialog.ShowDialog();

            if (result == true)
            {
                newTodo = new Todo();
                newTodo.Title = dialog.TitleText;
                newTodo.Description = dialog.DescriptionText;

                return true;
            }
            newTodo = null;
            return false;
        }
        private async Task LoadTodoListFromDynamoDB()
        {
            GetItemRequest request = new GetItemRequest
            {
                TableName = "todos",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "userid", new AttributeValue { S = UserSession.UserId } }
                }
            };

            GetItemResponse response = await _client.GetItemAsync(request);


            string todoListJson = response.Item["TodoListJson"].S;
            TodoList = JsonConvert.DeserializeObject<ObservableCollection<Todo>>(todoListJson);
            OnPropertyChanged(nameof(TodoList));
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTodo != null)
            {

                var dialog = new EditTodoDialogWindow();
                dialog.DataContext = dialog;

                dialog.TitleText = SelectedTodo.Title;
                dialog.DescriptionText = SelectedTodo.Description;


                var result = dialog.ShowDialog();


                if (result == true)
                {

                    SelectedTodo.Title = dialog.TitleText;
                    SelectedTodo.Description = dialog.DescriptionText;

 
                    await UpdateTodoItemInDynamoDB(SelectedTodo);

                    RefreshTodoList();
                }
            }
        }

        private async Task UpdateTodoItemInDynamoDB(Todo todo)
        {

            string todoListJson = JsonConvert.SerializeObject(TodoList);

  
            UpdateItemRequest request = new UpdateItemRequest
            {
                TableName = "todos",
                Key = new Dictionary<string, AttributeValue>
        {
            { "userid", new AttributeValue { S = UserSession.UserId } } 
        },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":val", new AttributeValue { S = todoListJson } }
        },
                UpdateExpression = "SET TodoListJson = :val"
            };


            await _client.UpdateItemAsync(request);
        }


        private void RefreshTodoList()
        {

            var updatedTodoList = new ObservableCollection<Todo>(TodoList);


            TodoList.Clear();


            foreach (var todo in updatedTodoList)
            {
                TodoList.Add(todo);
            }
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {

            if (SelectedTodo != null)
            {

                TodoList.Remove(SelectedTodo);

   
                await UpdateTodoItemInDynamoDB(SelectedTodo);
            }
        }
    }
}
