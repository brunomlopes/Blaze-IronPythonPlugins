$login = "brunomlopes"
$repo = "Blaze-IronPythonPlugins"
$api_key = get-content "api.key"

$full_repo = $login+"/"+$repo

$filename = "e:\temp.txt"
$description = "This is a test file!"
$file = get-item $filename

$assembly = [System.Reflection.Assembly]::LoadFrom((get-item "lib\Krystalware.UploadHelper.dll"))

$downloads_path = "http://github.com/"+$full_repo+"/downloads"

$wc = new-object net.webclient
$upload_info = $wc.DownloadString($downloads_path)



$post = new-object System.Collections.Specialized.NameValueCollection
$post.Add('login',$login)
$post.Add('token',$api_key)
$post.Add('file_size',$file.Length)
$post.Add('content_type',"application/octet-stream")
$post.Add('file_name',$file.Name)
$post.Add('description',$description)
$wc = new-object net.webclient
$upload_info = [xml][System.Text.Encoding]::ASCII.GetString($wc.UploadValues($downloads_path, $post))


$wc = new-object net.webclient
$post = new-object System.Collections.Specialized.NameValueCollection
$post.Add('FileName',$file.Name)
$post.Add('policy',$upload_info.hash.policy)
$post.Add('success_action_status',"201")
$post.Add('key',$upload_info.hash.prefix+$file.Name)
$post.Add('AWSAccessKeyId',$upload_info.hash.accesskeyid)
$post.Add('signature',$upload_info.hash.signature)
$post.Add('acl',$upload_info.hash.acl)

$upload_file = new-object Krystalware.UploadHelper.UploadFile $file.FullName, "file", "application/octet-stream" 
[Krystalware.UploadHelper.HttpUploadHelper]::Upload("http://github.s3.amazonaws.com/", $upload_file, $post)

#$result = [System.Text.Encoding]::ASCII.GetString($wc.UploadData("http://github.s3.amazonaws.com/"+$upload_info.hash.prefix+$file.Name,"PUT",$file_content))

#$e = $error[0]
#$s = $e.Exception.InnerException.Response.GetResponseStream()
#$readStream = new-object System.IO.StreamReader $s
#$readStream.ReadToEnd()