using System.Net.NetworkInformation;
using System.Text.Json;

namespace qmsUpdater
{
    public partial class Form1 : Form
    {
        private int token_number = 1;
        private string terminal_id = "biometric2";
        private string localIp = "127.0.0.1";
        HttpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8000/qms/");
            try
            {

                localIp = getIpAddress(NetworkInterfaceType.Wireless80211);
                label1.Text = localIp;
            }
            catch (Exception ex)
            {
                label1.Text = ex.Message;
            }

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            send_to_server("previous");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            send_to_server("next");
        }

        private async void send_to_server(string action)
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("terminal_id", terminal_id),
                new KeyValuePair<string, string>("token_number", token_number.ToString()),
                new KeyValuePair<string, string>("action", action)
            });
            try
            {
                var result = await client.PostAsync("terminal/update/display/", content);
                Response response = JsonSerializer.Deserialize<Response>(await result.Content.ReadAsStringAsync());
                if (response.success == true)
                {
                    token_number = response.token_number;
                    MessageBox.Show(response.token_number.ToString());
                }
                else
                {
                    MessageBox.Show(response.token_number.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send_to_server("repeat");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("terminal_id", terminal_id),
                new KeyValuePair<string, string>("ip_address", localIp),
            });
            try
            {
                var result = await client.PostAsync("terminal/connect/", content);
                Response response = JsonSerializer.Deserialize<Response>(await result.Content.ReadAsStringAsync());
                if (response.success == true)
                {
                    label1.Text = "Connected to server.";
                    token_number = response.token_number;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                label1.Text = "Check your app and database\n server are up running.";
            }
        }

        private String getIpAddress(NetworkInterfaceType type)
        {
            string ip_address = "0.0.0.0";
            foreach(NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = item.GetIPProperties();
                    if (adapterProperties.GatewayAddresses.FirstOrDefault()!= null)
                    {
                        foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                        {
                            if(ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ip_address = ip.Address.ToString();
                            }
                        }
                    }

                }
                
            }
            return ip_address;

        }
    }
}
