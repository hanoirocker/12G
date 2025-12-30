- Se comenzó a trabajar en la posibilidad de muerte del jugador. A analizar si vale la pena o no. Stasheado.

TOFIX:

- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales.

TODO - próximo:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún agregar lo que sucedería entre el final de Noises y el comienzo de Headaches donde choca el auto de la policia. Simon debería intentar contactarse con Mica luego de haber escuchado la conversación policial, pero sin lograrlo. Se podría mencionar el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Configurar linterna para que siempre esté activa en el inventario del jugador en la escena nocturna pero su luz sea de muy baja intensidad al estar oculta, y vaya subiendo a medida que se muestra. Esto quizas arregle el problema de tiempo de carga de shader al activarla/desactivarla en cambio.

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK:

- aumentar umbral de mareos?
