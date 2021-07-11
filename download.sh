#!/bin/bash
when=$(date)
base="https://celestrak.com/software/vallado"
names=("datalib" "fortran" "cpp" "pascal" "matlab" "cs")

echo "Fetching celestrak data (for %s)" "$when"

for name in "${names[@]}"; do
	echo "Fetching: $base/$name.zip"
	curl -fL "$base/$name.zip" > "$name.zip"
	rm -rf "$name"
	unzip "$name.zip"
	rm "$name.zip"
done

sgp4="https://celestrak.com/publications/AIAA/2008-6770/AIAA-2008-6770.zip"
echo "Fetching: $sgp4"
curl -fL "$sgp4" > "sgp4.zip"
rm -rf sgp4dc
unzip "sgp4.zip"
rm "sgp4.zip"

curl -fL "https://celestrak.com/software/vallado/AstroSoftware.xlsx" > AstroSoftware.xlsl

printf "Fetched on: %s\n" "$when" > when.txt

