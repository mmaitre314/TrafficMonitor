# Start 'Microsoft Azure PowerShell' (needed to get the right assemblies)
# Call Add-AzureAccount before to add account info
# Call 'Set-ExecutionPolicy Unrestricted' before running this script (elevated)

$connectionString = "DefaultEndpointsProtocol=https;AccountName=mmaitretrafficmonitor;AccountKey=supq0+mwYXTUuBhXJ99sfiCDKz+m/u8eVurzTv/SmOUE8E1dni1Zyu3yCCbkwXocM/rz/XVIUt4flw6LYKAZUA=="
$tableName = "Routes"
$outputFileName = $tableName + ".csv"

# From https://raw.githubusercontent.com/chriseyre2000/Powershell/master/Azure2/query-azuretable2.psm1
function EntityToObject ($item)
{
    $p = new-object PSObject
    $p | Add-Member -Name ETag -TypeName string -Value $item.ETag -MemberType NoteProperty 
    $p | Add-Member -Name PartitionKey -TypeName string -Value $item.PartitionKey -MemberType NoteProperty
    $p | Add-Member -Name RowKey -TypeName string -Value $item.RowKey -MemberType NoteProperty
    $p | Add-Member -Name Timestamp -TypeName datetime -Value $item.Timestamp -MemberType NoteProperty

    $item.Properties.Keys | foreach { 
        $type = $item.Properties[$_].PropertyType;
        $value = $item.Properties[$_].PropertyAsObject; 
        Add-Member -InputObject $p -Name $_ -Value $value -TypeName $type -MemberType NoteProperty -Force 
        }
    $p
}

Write-Host Connecting to Storage Table $tableName
$storageAccount = [Microsoft.WindowsAzure.Storage.CloudStorageAccount]::Parse($connectionString)
$tableClient = $storageAccount.CreateCloudTableClient()
$routeTable = $tableClient.GetTableReference($tableName)

Write-host Querying data
$query = New-Object "Microsoft.WindowsAzure.Storage.Table.TableQuery"
$data = $routeTable.ExecuteQuery($query)

Write-Host Exporting data to $outputFileName
$data | % { EntityToObject $_ } | Select-Object PartitionKey,Time,Duration,Distance | Export-Csv $outputFileName -NoTypeInformation
