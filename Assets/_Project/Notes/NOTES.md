Últimos cambios:
- Se cargan correctamente las escenas desde Nuevo Juego o Continuar
- Los eventos se actualizan y guardan correctamente

TOFIX:
- Si uno decide continuar el juego, actualmente se arranca a jugar desde el evento 0, no desde el último valor de savedEventIndex
- Luego de presionar cualquier tecla para arrancar la nueva escena, el Loading Scene Canvas desaparece y se ve la cámara del jugador por unos instantes.

TODO:

- REFACTOR GAMEMANAGER AND EVENTCONTROLLER: No puede haber tánta lógica distinta para ciclos tan similares. Encontrar alguna forma de Ejecutar los comandos
  del Event Controller pasando parámetros específicos desde el manager. 
- Implementar cambios de Video del Settings Menu

TOCHECK: