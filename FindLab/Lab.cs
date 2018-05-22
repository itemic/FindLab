using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.IO;
using System.Collections.ObjectModel;

namespace FindLab
{
    [DataContract]
    public class Lab
    {
        [DataMember]
        public int machines_in_use { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public object current_tutorial { get; set; }

        [DataMember]
        public object next_tutorial { get; set; }

        [DataMember]
        public int total_machines { get; set; }

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public bool closed { get; set; }

        [DataMember]
        public string timestamp { get; set; }

        [DataMember]
        public string room { get; set; }
        
        public string capitalizedRoomName
        {
            get
            {
                return room.ToUpper();
            }
        }

        public int availability
        {
            get
            {
                return total_machines - machines_in_use;
            }
        }

        public string availabilityString
        {
            get
            {
                return availability + " available";
            }
        }
    }

    class Proxy
    {
        public async static Task<List<Lab>> GetLabStatus(int labId)
        {
            var http = new HttpClient();
            var url = String.Format("http://www.fos.auckland.ac.nz/api/lab/current_usage/{0}.json", labId);
            System.Diagnostics.Debug.WriteLine(url);
            var response = await http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            var serializer = new DataContractJsonSerializer(typeof(List<Lab>));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (List<Lab>)serializer.ReadObject(stream);
            return data;
        }
    }

    public class LabViewModel
    {
        private Lab defaultLab = new Lab();
        public Lab DefaultLab { get { return this.defaultLab; } }
        private ObservableCollection<Lab> labs = new ObservableCollection<Lab>();
        public ObservableCollection<Lab> Labs { get { return this.labs; } }

        public LabViewModel() => LoadLabs();

        private async void LoadLabs()
        {
            List<int> labIds = new List<int>() { 1, 3, 5, 6, 7, 9, 11 };
            foreach (int id in labIds)
            {
                List<Lab> labToAdd = await Proxy.GetLabStatus(id);
                this.labs.Add(labToAdd[0]);
            }

        }
        
    }


}
