# UtmSchemaGrabber
Загрузчик XSD-схем из УТМ (Универсальный транспортный модуль) ЕГАИС

## Описание
Необходимые разработчикам XSD-схемы входят в состав УТМ и доступны по ссылкам в его веб-интерфейсе, при этом, однако, нет возможности загрузить их в одном месте в виде архива. UtmSchemaGrabber позволяет автоматизировать процесс загрузки и избавляет от необходимости скачивать файлы по одному вручную.

## Использование
```
UtmSchemaGrabber.exe http://<UtmAddress>:<port>
```
где:
\<UtmAddress\> - полный адрес УТМ, \<port\> - порт, на котором работает УТМ

Пример:
```
UtmSchemaGrabber.exe http://localhost:8080
```

## Архивы XSD-схем
Версия (тестовые УТМ)|Ссылка
---|---
1.0.12|https://github.com/dodbrian/UtmSchemaGrabber/raw/master/Schemas/EgaisXsd-1.0.12.zip
1.0.13|https://github.com/dodbrian/UtmSchemaGrabber/raw/master/Schemas/EgaisXsd-1.0.13.zip
1.0.14|https://github.com/dodbrian/UtmSchemaGrabber/raw/master/Schemas/EgaisXsd-1.0.14.zip
1.0.15|https://github.com/dodbrian/UtmSchemaGrabber/raw/master/Schemas/EgaisXsd-1.0.15.zip
1.0.16|https://github.com/dodbrian/UtmSchemaGrabber/raw/master/Schemas/EgaisXsd-1.0.16.zip
