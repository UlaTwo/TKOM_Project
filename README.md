# TKOM_Project
######Uruchamianie projektu:

w celu przetestowania działania leksera:
>dotnet test
po zmianie ścieżki na ./tkom.Test

w celu uruchomienia leksera:
>dotnet run -tlf ścieżka_do_pliku/nazwa_pliku.txt
lub
>dotnet run -tls string

Uruchomienie w trybie działania -tls skutkuje wypisaniem w konsoli poszczególnych tokenów wejściowego string'a.
Natomiast uruchomienie w trybie działania -tlf skutkuje utworzeniem pliku o nazwie nazwa_pliku_testLexer.txt w tej samej ścieżce, co plik wejściowy.
