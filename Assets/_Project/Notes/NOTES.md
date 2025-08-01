Últimos cambios:
- Se cargan correctamente las escenas y eventos desde Nuevo Juego o Continuar
- Los eventos se actualizan y guardan correctamente

TOFIX:
- No están ejecutandose las transiciones desde la Player VC hacia la BagVC y a la PhoneVC. Quizas falta especificar el tipo de transicion?

TODO:

- Implementar cambios de Video del Settings Menu

TOCHECK:

- Actualmente si se continua una partida anterior, se carga la escena y se ejecuta desde el evento guardado PERO .. los objetos alterados en eventos anteriores no aparecen en las nuevas ubicaciones. Vale la pena guarda la información de todos estos objetos y setearlos al iniciar la partida guardada? O lo mejor es guardar únicamente por escenas? Quizas rompiendo las escenas en otras escenas mas chicas a modo checkpoints mas cercanos con el Environment y sus objetos modificados ... 