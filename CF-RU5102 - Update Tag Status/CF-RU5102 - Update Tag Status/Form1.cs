using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CF_RU5102___Update_Tag_Status
{
    
    public partial class Form1 : Form
    {
        // Connection
        private static MongoClient client = new MongoClient("mongodb://localhost:27017");
        // Database
        private static IMongoDatabase database = client.GetDatabase("RFID");
        
        public Form1()
        {
            InitializeComponent();
            //insertTag();
            //updateTag(1000);
            updateManyTagsAsync();
        }

        private void insertTag()
        {
            var document = new BsonDocument
            {
                { "tagId", 1001 },
                { "longitude", 1000 },
                { "latitude", 1000 },
                { "locationId", 1120 },
                { "status", false },
                { "date", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") },
            };
            var collection = database.GetCollection<BsonDocument>("tags"); // Its automatically create table if it doesn't exist.
            collection.InsertOne(document);
            MessageBox.Show("New Record inserted");
        }

        private void updateTag(long tagId)
        {
            var collection = database.GetCollection<BsonDocument>("tags");
            // Find row
            var filter = Builders<BsonDocument>.Filter.Eq("tagId", tagId);
            // Set New data
            var update = Builders<BsonDocument>.Update.Set("status", false).Set("date", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            // Commit update
            collection.UpdateOneAsync(filter, update);
            MessageBox.Show("Updated!" + System.DateTime.Today.ToString());
        }

        private async Task updateManyTagsAsync() {
            var collection = database.GetCollection<BsonDocument>("tags");

            // our example list
            List<Tag> products = new List<Tag>();

            Tag t1 = new Tag(1000, 2, 3, 4, true, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            Tag t2 = new Tag(1001, 343, 334, 34, true, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            products.Add(t1);
            products.Add(t2);

            // initialise write model to hold list of our upsert tasks
            var models = new WriteModel<BsonDocument>[products.Count];

            // use ReplaceOneModel with property IsUpsert set to true to upsert whole documents
            for (var i = 0; i < products.Count; i++)
            {
                var bsonDoc = products[i].ToBsonDocument();
                models[i] = new ReplaceOneModel<BsonDocument>(new BsonDocument("tagId", products[i].tagId), bsonDoc) { IsUpsert = true };
            };

            await collection.BulkWriteAsync(models);
            MessageBox.Show("Bulk Updated!");
        }

    }
    class Tag {

        public Int64 tagId { get; set; }
        public Int64 longitude { get; set; }
        public Int64 latitude { get; set; }
        public Int64 locationId { get; set; }
        public bool status { get; set; }
        public String date { get; set; }

        public Tag(){}

        public Tag (Int64 tagId, Int64 longitude, Int64 latitude, Int64 locationId, bool status, String date) {
            this.tagId = tagId;
            this.longitude = longitude;
            this.latitude = latitude;
            this.locationId = locationId;
            this.status = status;
            this.date = date;
        }
    }
}
