namespace Lab;
public sealed record KeyRecord(DateTime Time,string Event,string Key)
{ public override string ToString() => $"{Time:yyyy-MM-dd HH:mm:ss.fff} | {Event,-8} | {Key}"; }
public sealed class MainForm:Form
{
 bool recording;
 readonly List<KeyRecord> records=new();
 readonly ListBox list=new(){Dock=DockStyle.Fill,HorizontalScrollbar=true};
 readonly Label status=Ui.Label("Kayıt durduruldu");
 public MainForm()
 {
  Ui.Setup(this,"Key Event Logger"); KeyPreview=true;
  var bar=Ui.Bar();
  bar.Controls.AddRange([Ui.Button("Başlat",(_,_)=>SetRecording(true)),Ui.Button("Durdur",(_,_)=>SetRecording(false)),Ui.Button("Temizle",(_,_)=>{records.Clear();list.Items.Clear();}),Ui.Button("TXT kaydet",(_,_)=>Save()),status]);
  var input=new TextBox{Dock=DockStyle.Bottom,Multiline=true,Height=110,PlaceholderText="Denemek için bu alana yazın. Yalnızca bu pencere odaktayken tuş olayları yakalanır."};
  var info=Ui.Label("Yerel eğitim uygulaması • Başlat’a basın ve deneme alanına yazın."); info.Dock=DockStyle.Bottom;
  Controls.Add(list); Controls.Add(input); Controls.Add(info); Controls.Add(bar);
  KeyDown+=(_,e)=>Record("KeyDown",e.KeyData.ToString()); KeyUp+=(_,e)=>Record("KeyUp",e.KeyData.ToString());
  Activated+=(_,_)=>RefreshStatus(); Deactivate+=(_,_)=>RefreshStatus();
 }
 void SetRecording(bool value) { recording=value; RefreshStatus(); }
 void RefreshStatus() { status.Text=recording?"● Kayıt açık — yalnızca bu pencere":"■ Kayıt durduruldu"; status.ForeColor=recording?Color.DarkGreen:Color.Firebrick; }
 void Record(string type,string key) { if(!recording||Form.ActiveForm!=this) return; var r=new KeyRecord(DateTime.Now,type,key); records.Add(r); list.Items.Add(r); list.TopIndex=list.Items.Count-1; }
 void Save() { using var dialog=new SaveFileDialog{Filter="Metin dosyası|*.txt",FileName="tus-kayitlari.txt"}; if(dialog.ShowDialog()!=DialogResult.OK)return; try { File.WriteAllLines(dialog.FileName,records.Select(x=>x.ToString()),System.Text.Encoding.UTF8); }catch(Exception e){Ui.Error(e);} }
}
