# TKOM_Project - kompilator języka umożliwiającego rysowanie obiektów graficznych
#### Uruchamianie projektu:

- Uruchomienie testów (po zmianie ścieżki na ./tkom.Test): 
  >dotnet test
  
- Uruchomienie interpretere (po zmianie ścieżki na ./tkom):
  >dotnet run -tif ścieżka_do_pliku/nazwa_pliku.txt


- Uruchomienie leksera (po zmianie ścieżki na ./tkom):
  >dotnet run -tlf ścieżka_do_pliku/nazwa_pliku.txt

  lub

  >dotnet run -tls string


- Uruchomienie parsera (po zmianie ścieżki na ./tkom):
  >dotnet run -tpf ścieżka_do_pliku/nazwa_pliku.txt

  lub

  >dotnet run -tps string


Uruchomienie w trybie działania -tls skutkuje wypisaniem w konsoli poszczególnych tokenów wejściowego string'a.

Natomiast uruchomienie w trybie działania -tlf skutkuje utworzeniem pliku o nazwie nazwa_pliku_testLexer.txt w tej samej ścieżce, co plik wejściowy.
