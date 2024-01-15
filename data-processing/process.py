import datetime
import json
import pandas as pd

def wrap(s, noTrailingComma = False):
    trailingComma = ","
    if(noTrailingComma):
        trailingComma = ""

    return "\"" + s + "\"" + trailingComma

# Opening JSON file
f = open('data.json')
  
data = json.load(f)
f.close()


datestr = datetime.datetime.now().__str__()
datestr = datestr.replace(":","_")
datestr = datestr.replace(" ","-")
fn = "processed_" + datestr + ".csv"

output_csv = open(fn, 'w')

header = wrap("timestamp") + wrap("participant_id") + wrap("technique") + wrap("gesture_set") \
    + wrap("pointer_type")+ wrap("height") + wrap("distance") \
    + wrap("checkpoint_time") + wrap("object_view_time") + wrap("total_time") \
    + wrap("mistake_count") + wrap("d_from_center") + wrap("d_from_center_with_spike") \
    + wrap("arm_distance") + wrap("arm_height") + wrap("wrist_angle") \
    + wrap("personalization_type")+ wrap("curveCombo")+ wrap("pointerHand") + wrap("confirmationType")+ wrap("targetSize") + wrap("combo_index")

output_csv.write(header + '\n')


csvRAW = ''
for participant_id in data:
    if("setting" in participant_id or "Timestamp" in participant_id):
        continue

    for ts in data[participant_id]:
        # print(ts)
        csvDataRow = data[participant_id][ts]
        csvRAW = csvRAW + csvDataRow + '\n'
        
        
        # output_csv.write()


# output_csv.close()

filedata = csvRAW

# Replace the target string
# filedata = filedata.replace('C1', 'CA')
# filedata = filedata.replace('C2', 'CB')
# filedata = filedata.replace('C3', 'CC')
# filedata = filedata.replace('C4', 'CD')

# filedata = filedata.replace('CA', 'C3')
# filedata = filedata.replace('CB', 'C1')
# filedata = filedata.replace('CC', 'C4')
# filedata = filedata.replace('CD', 'C2')


output_csv.write(filedata)
output_csv.close()

# df = pd.read_csv(fn)

# output1 = df[['curveCombo','checkpoint_time']]\
# .groupby(['curveCombo' ]).mean('checkpoint_time')

# output2 = df[['Participant_id', 'curveCombo','checkpoint_time']]\
# .groupby(['Participant_id', 'curveCombo' ]).mean('checkpoint_time')


# output3 = df[['Participant_id', 'curveCombo','checkpoint_time']]\
# .groupby(['Participant_id', 'curveCombo' ]).quantile([.10,0.50,0.90]).unstack(level=1)



# sheet_name = "results_summary"
# writer = pd.ExcelWriter("results_summary_" + datestr + ".xlsx",engine='xlsxwriter')   
# workbook=writer.book
# worksheet=workbook.add_worksheet(sheet_name)
# writer.sheets[sheet_name] = worksheet

# output1.to_excel(writer,sheet_name=sheet_name,startrow=0 , startcol=0)   
# output2.to_excel(writer,sheet_name=sheet_name,startrow=len(output1.index)+5, startcol=0) 
# output3.to_excel(writer,sheet_name=sheet_name,startrow=len(output1.index)+ 5 + len(output2.index)+5, startcol=0) 
# writer.close()
