- Se reorganizaron en el editor las configuraciones de creación de assets (Scriptable Objects). Ahora todas están dentro de Create -> SO's
- Se creó el asset CheckpointProfileSO con la intención de guardar todas las configuraciones necesarias para cargar al iniciar desde un checkpoint (Lógica no aplicada aún). * SE DECIDIO NO TRABAJAR CHECKPOINTS COMO ESCENAS MAS CHICAS.
- Se modificó el PlayerInventory. Ahora todos los items están instanciados por defecto en mano izq o mano derecha, solo se activan o desactivan.

TOFIX:


TODO - próximo:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún agregar lo que sucedería entre el final de Noises y el comienzo de Headaches donde choca el auto de la policia. Simon debería intentar contactarse con Mica luego de haber escuchado la conversación policial, pero sin lograrlo. Se podría mencionar el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar cajas bajo la escalera, o algo.
- Continuar con evento Headaches: hasta ahora solo hay choque de policia.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Posiblidad de muerte por sobreexposición a la resonancia?
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

- SISTEMA GUARDADO POR CHECKPOINTS: Crear Checkpoint assets para cada evento en particular y setear configuraciones.

  - CHECKPOINT PizzaTime:
    - GiantWindow.001: recibe destroyWindowToReplace.Raise(this, null); que desactiva "Window To Replace" para activar "Replaced Window" y "ZoomBird".
    - Mostrar la mancha de sangre "Blood Stain" en la PanoramaWindow.
    - Desactivar la escoba de la cocina y la bolsa de basura del garage.
    - Activar la "Used - Broom". Esto quizas involucre modificar el PlayerInventory también.
    - Deshabilitar el Sphere Collider de Top de "Trash Can"

  - CHECKPOINT WakeUpAtNight:
    - Lluvia activada.
  - Luego de Locked Up:
    - Linterna habilitada.
    - Puerta de la pieza del jugador desbloqueada.
    - Electricidad caida. 

TOCHECK:

- aumentar umbral de mareos?
